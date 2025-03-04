using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    [Header("½ÇÉ«ÐÅÏ¢")]
    [SerializeField] private float movingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float presentHealth;

    private float currentMovingSpeed;
    private float characterHealth = 40f;

    [Header("Destination var")]
    [SerializeField] private Animator animator;
    [SerializeField] private List<Transform> wayPoints;
    private int currentWayPointIndex = 0;
    private bool movingForward = true;

    [Header("Guard AI")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float visionRadius;
    [SerializeField] private float shootingRadius;
    [SerializeField] private bool playerInVisionRadius;
    [SerializeField] private bool playerInShootingRadius;

    [Header("Guard Shooting Var")]
    [SerializeField] private float giveDamageOf = 3f;
    [SerializeField] private float shootingRange = 100f;
    [SerializeField] private GameObject shootingRaycastArea;
    [SerializeField] private float timebtwShoot;
    private bool previouslyShoot;

    [Header("Character Controller and Gravity")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float gravity = 9.81f;
    private Vector3 velocity;
    [SerializeField] private bool isAlerted = false;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        currentMovingSpeed = movingSpeed;
        presentHealth = characterHealth;
        playerBody = GameObject.Find("Player");
    }

    private void Update()
    {
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInShootingRadius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if (!isAlerted)
        {
            //walk Ñ²Âß
            Walk();
        }

        if (isAlerted && playerInVisionRadius && !playerInShootingRadius)
        {
            //×·ÖðÍæ¼Ò
        }

        if (isAlerted && playerInVisionRadius && playerInShootingRadius)
        {
            //Éä»÷Íæ¼Ò
        }

        if (isAlerted)
        {
            visionRadius += 30f;
        }
    }

    private void Walk()
    {
        if (wayPoints.Count == 0)
            return;

        Transform targetWayPoint = wayPoints[currentWayPointIndex];
        Vector3 directionToWayPoint = (targetWayPoint.position - transform.position).normalized;
        Vector3 moveVector = directionToWayPoint * movingSpeed * Time.deltaTime;

        characterController.Move(moveVector);

        Vector3 lookDirection = new Vector3(directionToWayPoint.x, 0, directionToWayPoint.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection),
            Time.deltaTime * turningSpeed);

        animator.SetBool("Run", false);
        animator.SetBool("Walk", true);
        animator.SetBool("Shoot", false);

        if (Vector3.Distance(transform.position, targetWayPoint.position) < 0.1f)
        {
            if (movingForward)
            {
                currentWayPointIndex++;
                if (currentWayPointIndex >= wayPoints.Count)
                {
                    currentWayPointIndex = wayPoints.Count - 1;
                    movingForward = false;
                }
            }
            else
            {
                currentWayPointIndex--;
                if (currentWayPointIndex < 0)
                {
                    currentWayPointIndex = 0;
                    movingForward = true;
                }
            }
        }
    }

}
