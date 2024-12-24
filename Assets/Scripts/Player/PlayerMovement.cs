using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("脚本引用")]
    private InputManager inputManager;

    [Header("移动")]
    private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    private Rigidbody playerRigidbody;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    //记录最后一次有效的移动方向
    private Vector3 lastMoveDirection;

    [Header("移动状态")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;

    [Header("重力")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallSpeed = 5f;


    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        // 计算基于摄像机方向的移动向量
        moveDirection = camObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + camObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;// 不沿着垂直方向移动

        //移动逻辑
        if (isRunning)
        {
            moveDirection = moveDirection * runningSpeed;
        }
        else
        {
            if (inputManager.moveAmount > 0.5f)
            {
                moveDirection = moveDirection * walkingSpeed;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        //当有输入时，更新 lastMoveDirection
        if (moveDirection.magnitude > 0)
        {
            lastMoveDirection = moveDirection;
        }

        // 使用 Rigidbody 来应用移动
        //Vector3 movementVelocity = moveDirection;
        //movementVelocity.y = playerRigidbody.velocity.y;
        //playerRigidbody.velocity = movementVelocity;

        //只更新 X 和 Z 方向的速度，保留了当前的 Y 轴速度
        playerRigidbody.velocity = new Vector3(moveDirection.x, playerRigidbody.velocity.y, moveDirection.z);
    }

    private void HandleRotation()
    {
        //Vector3 targetDirection = Vector3.zero;

        //targetDirection = camObject.forward * inputManager.verticalInput;
        //targetDirection = targetDirection + camObject.right * inputManager.horizontalInput;
        //targetDirection.Normalize();
        //targetDirection.y = 0;

        // 如果有输入，旋转到当前移动方向；否则保持最后的移动方向
        Vector3 targetDirection = moveDirection.magnitude > 0 ? moveDirection : lastMoveDirection;

        // 没有有效方向时，不更新旋转
        if (targetDirection.magnitude == 0)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void ApplyGravity()
    {
        if (isGrounded == false) //玩家不在地面时
        {
            Vector3 currentVelocity = playerRigidbody.velocity;

            // 计算目标垂直速度
            float targetYVelocity = gravity * fallSpeed * Time.deltaTime;

            // 使用 Mathf.Lerp 平滑过渡当前速度与目标速度
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetYVelocity, 0.1f); // 0.1f 控制过渡速率

            playerRigidbody.velocity = currentVelocity;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 仅当接触地面时
        {
            isGrounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 仅当接触地面时
        {
            isGrounded = false;
        }
    }
}
