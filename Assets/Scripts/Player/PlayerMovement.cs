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
    private Rigidbody playerRigidbody;

    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float rotationSpeed = 12f;

    //��¼���һ����Ч���ƶ�����
    private Vector3 lastMoveDirection; 


    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void HandleAllMovement()
    {
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
        moveDirection = moveDirection * movementSpeed;

        //��������ʱ������ lastMoveDirection
        if (moveDirection.magnitude > 0)
        {
            lastMoveDirection = moveDirection;
        }

        // ʹ�� Rigidbody ��Ӧ���ƶ�
        //Vector3 movementVelocity = moveDirection;
        //playerRigidbody.velocity = movementVelocity;

        //ֻ���� X �� Z ������ٶȣ������˵�ǰ�� Y ���ٶ�
        playerRigidbody.velocity = new Vector3(moveDirection.x, playerRigidbody.velocity.y, moveDirection.z);
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
}
