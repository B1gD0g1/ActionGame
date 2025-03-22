using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    [Header("角色信息")]
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
    private Vector3 velocity;
    [SerializeField] private bool isAlerted = false;

    [SerializeField] private GameObject alertUI;
    [SerializeField] private GameObject cautionUI;
    private DeadBodyPick deadBodyPick;

    [Header("脚步声音")]
    private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepIntercal = 0.5f;
    private float footstepTimer;

    [Header("特效")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bloodEffect;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        deadBodyPick = GetComponent<DeadBodyPick>();
        deadBodyPick.enabled = false;
        footstepAudioSource = GetComponent<AudioSource>();
        footstepAudioSource.rolloffMode = AudioRolloffMode.Linear;

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
            //walk 巡逻
            Walk();
        }

        if (isAlerted && playerInVisionRadius && !playerInShootingRadius)
        {
            //追逐玩家
            ChasePlayer();
        }

        if (isAlerted && playerInVisionRadius && playerInShootingRadius)
        {
            //射击玩家
            ShootPlayer();
        }

        if (isAlerted)
        {
            alertUI.SetActive(true);
            cautionUI.SetActive(false);
            visionRadius = 30f;
        }
    }

    public void AlertGuard()
    {
        isAlerted = true;
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

        HandleFootstepSound();

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

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (playerBody.transform.position - transform.position).normalized;
        Vector3 moveVector = directionToPlayer * currentMovingSpeed * Time.deltaTime;

        characterController.Move(moveVector);

        Vector3 lookDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection),
            Time.deltaTime * turningSpeed);

        animator.SetBool("Run", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Shoot", false);

        currentMovingSpeed = runningSpeed;
        HandleFootstepSound();
    }

    private void ShootPlayer()
    {
        currentMovingSpeed = 0f;

        Vector3 directionToPlayer = (playerBody.transform.position - transform.position).normalized;

        Vector3 lookDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection),
            Time.deltaTime * turningSpeed);


        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Shoot", true);

        if (!previouslyShoot)
        {
            RaycastHit hit;
            if (Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward,
                out hit, shootingRange))
            {
                Debug.Log("Guard Hit" + hit.transform.name);

                PlayerMovement player = hit.transform.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    //枪口火光
                    muzzleFlash.Play();

                    player.CharacterHitDamage(giveDamageOf);

                    //血液效果
                    GameObject bloodEffectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bloodEffectGo, 0.5f);
                }
            }

            previouslyShoot = true;
            Invoke(nameof(ActiveShooting), timebtwShoot);
        }
    }

    private void ActiveShooting()
    {
        previouslyShoot = false;
    }

    public void CharacterHitDamage(float takeDamage)
    {
        visionRadius = 155f;
        isAlerted = true;
        presentHealth -= takeDamage;

        if (presentHealth <= 0)
        {
            animator.SetBool("Die", true);
            CharacterDie();
        }
    }

    private void CharacterDie()
    {
        currentMovingSpeed = 0f;
        shootingRange = 0f;

        //hide guard body 隐藏尸体

        this.enabled = false;
        deadBodyPick.enabled = true;

        //disable the UI 
        alertUI.SetActive(false);
        cautionUI.SetActive(false);
    }

    private void HandleFootstepSound()
    {
        if (footstepAudioSource != null && footstepClip != null)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepAudioSource.PlayOneShot(footstepClip);
                footstepTimer = footstepIntercal;
            }
        }
    }
}
