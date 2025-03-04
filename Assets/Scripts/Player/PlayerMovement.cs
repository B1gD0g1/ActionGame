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
    private Animator animator;
    [SerializeField] private CameraManager cameraManager;


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
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float fallingSpeed;

    //记录最后一次有效的移动方向
    //private Vector3 lastMoveDirection;

    [Header("状态")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public bool isOnObstacle;
    public bool isJumping = false;
    public bool isSlope;
    public bool isScoped;

    public bool IsHanging { get; set; }

    public bool InAction { get; private set; }

    public bool IsOnLedge { get; set; }
    public LedgeInfo LedgeInfo { get; set; }

    private bool hasControl = true;


    [Header("检测地面")]
    [SerializeField] private float surfaceCheckRadius = 0.3f;
    [SerializeField] private Vector3 surfaceCheckOffset;
    [SerializeField] private LayerMask surfaceAndObstacleLayer;
    [SerializeField] private LayerMask obstacleLayer;
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
        animator = GetComponent<Animator>();
    }

    public void HandleAllMovement()
    {
        if (hasControl == false)
            return;

        if (IsHanging)
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
        animatorManager.SetBoolAnimator("isGrounded", (isOnObstacle || isGrounded));
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

        if (isScoped)
        {
            transform.rotation = Quaternion.Euler(cameraManager.GetPivotAngle(), 
                cameraManager.GetLookAngle(), 0);
        }
        else
        {
            transform.rotation = playerRotation;
        }
    }

    private void ApplyGravity()
    {
        // 只有在角色不在地面时才应用重力
        if (isGrounded == false && isOnObstacle == false)
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

            if (isOnObstacle && isRunning == false)
            {
                IsOnLedge = environmentCheck.CheckLedge(desiredMoveDirection, out LedgeInfo ledgeInfo);

                if (IsOnLedge)
                {
                    LedgeInfo = ledgeInfo;
                    LedgeMovement();
                    //Debug.Log("Player is on ledge");
                }
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

    public IEnumerator DoAction(string animationName, MatchTargetParams matchTargetParams = null,
        Quaternion targetRotation = new Quaternion(), bool rotate = false, float postActionDelay = 0)
    {
        InAction = true;

        // 执行攀爬动画
        animator.CrossFadeInFixedTime(animationName, 0.2f);
        yield return null;

        // 通知输入管理器开始攀爬
        inputManager.SetJumpingState(true);

        var animatorState = animator.GetNextAnimatorStateInfo(0);
        if (animatorState.IsName(animationName) == false)
        {
            Debug.LogError("动作名称不匹配！");
        }

        float rotationStartTime = (matchTargetParams != null) ? matchTargetParams.matchStartTime : 0f;

        float timer = 0f;
        while (timer <= animatorState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animatorState.length;

            if (rotate && normalizedTime > rotationStartTime)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    targetRotation,
                    RotationSpeed * Time.deltaTime);
            }

            if (matchTargetParams != null)
            {
                MatchTarget(matchTargetParams);
            }

            if (animator.IsInTransition(0) && timer > 0.5f)
            {
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(postActionDelay);

        // 完成攀爬后，恢复正常输入模式
        inputManager.SetJumpingState(false);

        InAction = false;
    }

    private void MatchTarget(MatchTargetParams mp)
    {
        if (animator.isMatchingTarget)
        {
            return;
        }

        animator.MatchTarget(mp.matchPosition,
            transform.rotation,
            mp.matchBodyPart,
            new MatchTargetWeightMask(mp.matchPositionWeight, 0),
            mp.matchStartTime,
            mp.matchTargetTime);
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

    public void EnableCharacterController(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void ResetTargetRotation()
    {
        targetRotation = transform.rotation;
    }

    //地面检测
    private void surfaceCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset),
            surfaceCheckRadius, surfaceLayer);

        isOnObstacle = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset),
            surfaceCheckRadius, obstacleLayer);

        SlopCheck();
    }

    //斜坡检测
    private void SlopCheck()
    {
        if (isGrounded)
        {
            RaycastHit groundHit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0),
                Vector3.down, out groundHit,
                slopeForceRayLength, surfaceAndObstacleLayer))
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


public class MatchTargetParams
{
    public Vector3 matchPosition;
    public AvatarTarget matchBodyPart;
    public Vector3 matchPositionWeight;
    public float matchStartTime;
    public float matchTargetTime;
}