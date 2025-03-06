using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkCharacterAI : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;
    private DeadBodyPick deadBodyPick;
    private CharacterController characterController;


    [SerializeField] private float walkSpeed;
    [SerializeField] private float health;
    private CapsuleCollider capsuleCollider;
    private bool isMoving = false;



    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();
        deadBodyPick = GetComponent<DeadBodyPick>();
        deadBodyPick.enabled = false;
        characterController.enabled = false;
    }

    private void Update()
    {
        if (!isMoving || navAgent.remainingDistance < 0.1f)
        {
            WalkToRandomWayPoint();
        }
    }

    private void WalkToRandomWayPoint()
    {
        navAgent.speed = walkSpeed;
        Vector3 randomPosition = RandomNavMeshPosition();

        navAgent.SetDestination(randomPosition);
        isMoving = true;
    }

    private Vector3 RandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 15f;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 15f, NavMesh.AllAreas);
        return hit.position;
    }

    public void CharacterHitDamage(float takeDamage)
    {
        
        health -= takeDamage;

        if (health <= 0)
        {
            animator.SetBool("isDead", true);
            CharacterDie();
        }
    }

    private void CharacterDie()
    {
        this.enabled = false;
        deadBodyPick.enabled = true;
        navAgent.enabled = false;
        capsuleCollider.enabled = false;
        characterController.enabled = true;
    }
}
