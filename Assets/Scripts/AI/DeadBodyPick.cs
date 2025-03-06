using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBodyPick : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerMovement playerMovement;
    private CharacterController characterController;
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Transform player;
    [SerializeField] private Transform deadBodyPickArea;
    [SerializeField] private Transform handPosition;
    [SerializeField] private float pickUpRange;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float fallingSpeedMultiplier = 2f;

    //private bool isPicking = false;
    private Vector3 velocity;


    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= pickUpRange 
            && inputManager.GetFInput())
        {
            if (playerMovement.isPicking)
            {
                DetachBody();
            }
            else
            {
                AttachBody();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!playerMovement.isPicking && characterController.enabled)
        {
            if (!characterController.isGrounded)
            {
                velocity.y -= gravity * fallingSpeedMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y = 0f;
            }

            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void AttachBody()
    {
        playerMovement.isPicking = true;
        characterController.enabled = false;
        transform.position = handPosition.position - (deadBodyPickArea.position - transform.position);
        transform.parent = player;

        transform.position = handPosition.position;
        transform.rotation = handPosition.rotation;

        velocity = Vector3.zero;

        playerAnimator.Play("DeadBodyCarry");
    }

    private void DetachBody()
    {
        playerMovement.isPicking = false;
        characterController.enabled = true;
        transform.parent = null;

        playerAnimator.Play("Basic Locomotion");
    }
}
