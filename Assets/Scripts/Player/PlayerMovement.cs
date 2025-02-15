using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("脚本引用")]
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    private EnvironmentCheck environmentCheck;

    [Header("移动")]
    private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    private CharacterController playerController;
    [SerializeField] private Vector3 velocity; // 用于保存角色当前的速度
    //记录垂直速度
    private float ySpeed;


    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    //记录最后一次有效的移动方向
    private Vector3 lastMoveDirection;

    [Header("状态")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public bool isClimbing = false;

    public bool playerOnLedge { get; set; }
    public LedgeInfo ledgeInfo { get; set; }

    private bool hasControl = true;


    [Header("检测地面")]
    [SerializeField] private float surfaceCheckRadius = 0.3f;
    [SerializeField] private Vector3 surfaceCheckOffset;
    [SerializeField] private LayerMask surfaceLayer;
    //private bool onSurface;


    private Quaternion targetRotation;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<CharacterController>();
        animatorManager = GetComponent<AnimatorManager>();
        environmentCheck = GetComponent<EnvironmentCheck>();
    }

    public void HandleAllMovement()
    {
        if (hasControl == false)
            return;
        
        HandleMovement();
        HandleRotation();

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

        surfaceCheck();
        //animatorManager.SetBoolAnimator("onSurface", isGrounded);
        //Debug.Log("isGround = " + isGrounded);
        ApplyGravity();

        velocity = new Vector3(moveDirection.x, ySpeed, moveDirection.z); // 更新水平速度 保持垂直速度

        // 使用 CharacterController 的 Move 函数来移动角色
        playerController.Move(velocity * Time.deltaTime); 
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

        targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void ApplyGravity()
    {
        // 只有在角色不在地面时才应用重力
        if (isGrounded == false)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            // 如果在地面上，重置垂直速度，防止角色被卡住
            ySpeed = -0.5f;

            playerOnLedge = environmentCheck.CheckLedge(moveDirection, out LedgeInfo ledgeInfo);

            if (playerOnLedge)
            {
                this.ledgeInfo = ledgeInfo;
                Debug.Log("Player is on ledge");
            }
        }

        //更新垂直方向的速度
        velocity.y = ySpeed;
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        
        playerController.enabled = hasControl;

        if (hasControl == false)
        {
            animatorManager.SetFloatAnimator("Vertical", 0f);
            animatorManager.SetFloatAnimator("Horizontal", 0f);
            targetRotation = transform.rotation;
        }
    }

    //地面检测
    private void surfaceCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), 
            surfaceCheckRadius, surfaceLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
}
