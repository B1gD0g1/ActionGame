using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ű�����")]
    private InputManager inputManager;
    private AnimatorManager animatorManager;
    private EnvironmentCheck environmentCheck;

    [Header("�ƶ�")]
    private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    private CharacterController playerController;
    [SerializeField] private Vector3 velocity; // ���ڱ����ɫ��ǰ���ٶ�
    //��¼��ֱ�ٶ�
    private float ySpeed;


    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    //��¼���һ����Ч���ƶ�����
    private Vector3 lastMoveDirection;

    [Header("״̬")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public bool isClimbing = false;

    public bool playerOnLedge { get; set; }
    public LedgeInfo ledgeInfo { get; set; }

    private bool hasControl = true;


    [Header("������")]
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
        // ������������������ƶ�����
        moveDirection = camObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + camObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;// �����Ŵ�ֱ�����ƶ�

        //�ƶ��߼�
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

        //��������ʱ������ lastMoveDirection
        if (moveDirection.magnitude > 0)
        {
            lastMoveDirection = moveDirection;
        }

        surfaceCheck();
        //animatorManager.SetBoolAnimator("onSurface", isGrounded);
        //Debug.Log("isGround = " + isGrounded);
        ApplyGravity();

        velocity = new Vector3(moveDirection.x, ySpeed, moveDirection.z); // ����ˮƽ�ٶ� ���ִ�ֱ�ٶ�

        // ʹ�� CharacterController �� Move �������ƶ���ɫ
        playerController.Move(velocity * Time.deltaTime); 
    }

    private void HandleRotation()
    {
        //Vector3 targetDirection = Vector3.zero;

        //targetDirection = camObject.forward * inputManager.verticalInput;
        //targetDirection = targetDirection + camObject.right * inputManager.horizontalInput;
        //targetDirection.Normalize();
        //targetDirection.y = 0;

        // ��������룬��ת����ǰ�ƶ����򣻷��򱣳������ƶ�����
        Vector3 targetDirection = moveDirection.magnitude > 0 ? moveDirection : lastMoveDirection;

        // û����Ч����ʱ����������ת
        if (targetDirection.magnitude == 0)
            return;

        targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void ApplyGravity()
    {
        // ֻ���ڽ�ɫ���ڵ���ʱ��Ӧ������
        if (isGrounded == false)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            // ����ڵ����ϣ����ô�ֱ�ٶȣ���ֹ��ɫ����ס
            ySpeed = -0.5f;

            playerOnLedge = environmentCheck.CheckLedge(moveDirection, out LedgeInfo ledgeInfo);

            if (playerOnLedge)
            {
                this.ledgeInfo = ledgeInfo;
                Debug.Log("Player is on ledge");
            }
        }

        //���´�ֱ������ٶ�
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

    //������
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
