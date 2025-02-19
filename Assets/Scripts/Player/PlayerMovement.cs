using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ű�����")]
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    private EnvironmentCheck environmentCheck;
    private CharacterController characterController;

    [Header("�ƶ�")]
    [SerializeField] private Vector3 desiredMoveDirection;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    private CharacterController playerController;
    [SerializeField] private Vector3 velocity; // ���ڱ����ɫ��ǰ���ٶ�
    //��¼��ֱ�ٶ�
    private float ySpeed;


    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float fallingSpeed;

    //��¼���һ����Ч���ƶ�����
    //private Vector3 lastMoveDirection;

    [Header("״̬")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public bool isClimbing = false;
    public bool isSlope;


    public bool IsOnLedge { get; set; }
    public LedgeInfo LedgeInfo { get; set; }

    private bool hasControl = true;


    [Header("������")]
    [SerializeField] private float surfaceCheckRadius = 0.3f;
    [SerializeField] private Vector3 surfaceCheckOffset;
    [SerializeField] private LayerMask surfaceLayer;

    [Header("б�¼��")]
    [SerializeField] private float slopeForceRayLength;

    [Header("��������")]
    [SerializeField] private bool isGroundedBuffer = false;
    [SerializeField] private float groundedBufferTime; // ����ʱ��
    [SerializeField] private float groundedBufferTimer = 0f;  // ��ǰ�����ʱ��


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
        // ������������������ƶ�����
        desiredMoveDirection = camObject.forward * inputManager.verticalInput;
        desiredMoveDirection += camObject.right * inputManager.horizontalInput;
        desiredMoveDirection.Normalize();
        desiredMoveDirection.y = 0;// �����Ŵ�ֱ�����ƶ�

        moveDirection = desiredMoveDirection;

        //�ƶ��߼�
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

        //velocity = new Vector3(moveDirection.x, ySpeed, moveDirection.z); // ����ˮƽ�ٶ� ���ִ�ֱ�ٶ�

        // ʹ�� CharacterController �� Move �������ƶ���ɫ
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
        // ֻ���ڽ�ɫ���ڵ���ʱ��Ӧ������
        if (isGrounded == false)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * walkingSpeed / 2;
        }
        else
        {
            // ����ڵ����ϣ����ô�ֱ�ٶȣ���ֹ��ɫ����ס
            if (isSlope == true)
            {
                ySpeed = Physics.gravity.y * fallingSpeed * Time.deltaTime;
            }
            else
            {
                ySpeed = -0.5f;
            }

            //����ˮƽ�ٶ� ���ִ�ֱ�ٶ�
            velocity = new Vector3(desiredMoveDirection.x, ySpeed, desiredMoveDirection.z);

            IsOnLedge = environmentCheck.CheckLedge(desiredMoveDirection, out LedgeInfo ledgeInfo);

            if (IsOnLedge)
            {
                LedgeInfo = ledgeInfo;
                LedgeMovement();
                //Debug.Log("Player is on ledge");
            }
        }

        //���´�ֱ������ٶ�
        velocity.y = ySpeed;
    }

    //�ϰ����ϵ��ƶ�����
    private void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeInfo.surfaceHit.normal,
            desiredMoveDirection, Vector3.up);
        var angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDirection, transform.forward) >= 70)
        {
            //�����ƶ���������ת
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
            //�Ƕ�С��90�����60�� ���ٶ�������ˮƽ������
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

    //������
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
        //        isGrounded = true; // �ڻ���ʱ������Ȼ��Ϊ��ɫ�ڵ�����
        //    }
        //}
    }

    //б�¼��
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
