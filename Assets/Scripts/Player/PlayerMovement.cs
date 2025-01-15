using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ű�����")]
    private InputManager inputManager;

    [Header("�ƶ�")]
    private Vector3 moveDirection;
    [SerializeField] private Transform camObject;
    //private Rigidbody playerRigidbody;
    private CharacterController playerController;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    //��¼���һ����Ч���ƶ�����
    private Vector3 lastMoveDirection;

    [Header("�ƶ�״̬")]
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;

    [Header("����")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallingSpeed = 5f;

    [Header("ˮƽ��ֱ�����ٶ�")]
    [SerializeField]private Vector3 velocity; // ���ڱ����ɫ��ǰ���ٶ�

    [Header("������")]
    [SerializeField] private float surfaceCheckRadius = 0.3f;
    [SerializeField] private Vector3 surfaceCheckOffset;
    [SerializeField] private LayerMask surfaceLayer;
    private bool onSurface;



    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<CharacterController>();
        //playerRigidbody = GetComponent<Rigidbody>();
    }

    public void HandleAllMovement()
    {
        ApplyGravity();
        HandleMovement();
        HandleRotation();
        surfaceCheck();
        Debug.Log("����ڵ���" + onSurface);
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

        // ʹ�� Rigidbody ��Ӧ���ƶ�
        //Vector3 movementVelocity = moveDirection;
        //movementVelocity.y = playerRigidbody.velocity.y;
        //playerRigidbody.velocity = movementVelocity;

        //ֻ���� X �� Z ������ٶȣ������˵�ǰ�� Y ���ٶ�
        //playerRigidbody.velocity = new Vector3(moveDirection.x, playerRigidbody.velocity.y, moveDirection.z);


        velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z); // ����ˮƽ�ٶ� ���ִ�ֱ�ٶ�

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

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void ApplyGravity()
    {
        // ֻ���ڽ�ɫ���ڵ���ʱ��Ӧ������
        if (!onSurface)
        {
            fallingSpeed += gravity * Time.deltaTime;

            //velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            // ����ڵ����ϣ����ô�ֱ�ٶȣ���ֹ��ɫ����ס
            fallingSpeed = 0f;
            //if (velocity.y < 0)
            //{
            //    velocity.y = 0f; // ����һ����С�ĸ�ֵ����ֹ��ɫ�ڵ���������
            //}
        }

        //���´�ֱ������ٶ�
        velocity.y = fallingSpeed;

        // ʹ�� CharacterController �� Move ���ƶ���ɫ
        //playerController.Move(velocity * Time.deltaTime);
    }

    private void surfaceCheck()
    {
        onSurface = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius, surfaceLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }

    //private void ApplyGravity()
    //{
    //    if (isGrounded == false) //��Ҳ��ڵ���ʱ
    //    {
    //        //Vector3 currentVelocity = playerRigidbody.velocity;

    //        // ����Ŀ�괹ֱ�ٶ�
    //        float targetYVelocity = gravity * fallSpeed * Time.deltaTime;

    //        // ʹ�� Mathf.Lerp ƽ�����ɵ�ǰ�ٶ���Ŀ���ٶ�
    //        //currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetYVelocity, 0.1f); // 0.1f ���ƹ�������
    //        //playerRigidbody.velocity = currentVelocity;

    //        // ʹ�� Mathf.Lerp ƽ�����ɵ�ǰ�ٶ���Ŀ���ٶ�
    //        velocity.y = Mathf.Lerp(velocity.y, targetYVelocity, 0.1f); // 0.1f ���ƹ�������
    //    }
    //    else
    //    {
    //        // �������ڵ����ϣ��������ô�ֱ�ٶ�
    //        if (velocity.y < 0)
    //        {
    //            velocity.y = -2f; // ����һ��С�ĸ�ֵ�����ֵ����ϵ��ٶ�
    //        }
    //    }
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground")) // �����Ӵ�����ʱ
    //    {
    //        isGrounded = true;
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground")) // �����Ӵ�����ʱ
    //    {
    //        isGrounded = false;
    //    }
    //}
}
