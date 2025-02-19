using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("脚本引用")]
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    private EnvironmentCheck environmentCheck;
    private CharacterController characterController;

    [Header("移动")]
    [SerializeField] private Vector3 desiredMoveDirection;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    private CharacterController playerController;
    [SerializeField] private Vector3 velocity; // 用于保存角色当前的速度
    //记录垂直速度
    private float ySpeed;


    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float fallingSpeed;

    //记录最后一次有效的移动方向
    //private Vector3 lastMoveDirection;

    [Header("状态")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public bool isClimbing = false;
    public bool isSlope;


    public bool IsOnLedge { get; set; }
    public LedgeInfo LedgeInfo { get; set; }

    private bool hasControl = true;


    [Header("检测地面")]
    [SerializeField] private float surfaceCheckRadius = 0.3f;
    [SerializeField] private Vector3 surfaceCheckOffset;
    [SerializeField] private LayerMask surfaceLayer;

    [Header("斜坡检测")]
    [SerializeField] private float slopeForceRayLength;

    [Header("重力缓冲")]
    [SerializeField] private bool isGroundedBuffer = false;
    [SerializeField] private float groundedBufferTime; // 缓冲时间
    [SerializeField] private float groundedBufferTimer = 0f;  // 当前缓冲计时器


    private Quaternion targetRotation;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<CharacterController>();
        animatorManager = GetComponent<AnimatorManager>();
        environmentCheck = GetComponent<EnvironmentCheck>();
        characterController = GetComponent<CharacterController>();
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
        desiredMoveDirection = camObject.forward * inputManager.verticalInput;
        desiredMoveDirection += camObject.right * inputManager.horizontalInput;
        desiredMoveDirection.Normalize();
        desiredMoveDirection.y = 0;// 不沿着垂直方向移动

        moveDirection = desiredMoveDirection;

        //移动逻辑
        if (isRunning)
        {
            desiredMoveDirection *= runningSpeed;
        }
        else
        {
            if (inputManager.moveAmount > 0.5f)
            {
                desiredMoveDirection *= walkingSpeed;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        surfaceCheck();
        animatorManager.SetBoolAnimator("isGrounded", isGrounded);
        ApplyGravity();

        //velocity = new Vector3(moveDirection.x, ySpeed, moveDirection.z); // 更新水平速度 保持垂直速度

        // 使用 CharacterController 的 Move 函数来移动角色
        playerController.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (inputManager.moveAmount > 0 && moveDirection.magnitude > 0.2f)
        {
            //targetDirection = moveDirection;
            targetRotation = Quaternion.LookRotation(moveDirection);
        }

        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation,
            rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void ApplyGravity()
    {
        // 只有在角色不在地面时才应用重力
        if (isGrounded == false)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * walkingSpeed / 2;
        }
        else
        {
            // 如果在地面上，重置垂直速度，防止角色被卡住
            if (isSlope == true)
            {
                ySpeed = Physics.gravity.y * fallingSpeed * Time.deltaTime;
            }
            else
            {
                ySpeed = -0.5f;
            }

            //更新水平速度 保持垂直速度
            velocity = new Vector3(desiredMoveDirection.x, ySpeed, desiredMoveDirection.z);

            IsOnLedge = environmentCheck.CheckLedge(desiredMoveDirection, out LedgeInfo ledgeInfo);

            if (IsOnLedge)
            {
                LedgeInfo = ledgeInfo;
                LedgeMovement();
                //Debug.Log("Player is on ledge");
            }
        }

        //更新垂直方向的速度
        velocity.y = ySpeed;
    }

    //障碍物上的移动输入
    private void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeInfo.surfaceHit.normal,
            desiredMoveDirection, Vector3.up);
        var angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDirection, transform.forward) >= 70)
        {
            //不能移动，可以旋转
            velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
        else if (angle < 90)
        {
            //角度小于90°大于60° 把速度限制在水平方向上
            var left = Vector3.Cross(Vector3.up, LedgeInfo.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * dir;
            moveDirection = dir;
        }

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

        SlopCheck();

        //if (isGrounded)
        //{
        //    isGroundedBuffer = false;
        //    groundedBufferTimer = 0f;
        //}
        //else
        //{
        //    isGroundedBuffer = true;
        //    groundedBufferTimer += Time.deltaTime;
        //    if (groundedBufferTimer < groundedBufferTime)
        //    {
        //        isGrounded = true; // 在缓冲时间内仍然认为角色在地面上
        //    }
        //}
    }

    //斜坡检测
    private void SlopCheck()
    {
        if (isGrounded)
        {
            RaycastHit groundHit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0),
                Vector3.down, out groundHit,
                slopeForceRayLength, surfaceLayer))
            {

                float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);

                if (groundAngle > 0 && groundAngle < 80)
                {
                    isSlope = true;
                }
                else
                {
                    isSlope = false;
                }
            }
            else
            {
                isSlope = false;
            }

            Debug.DrawLine(transform.position + new Vector3(0, 0.2f, 0),
                transform.position + Vector3.up + Vector3.down * slopeForceRayLength,
                isSlope ? Color.blue : Color.cyan);
        }
    }

    public bool HasControl
    {
        get => hasControl;
        set => hasControl = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float RotationSpeed => rotationSpeed;
}
