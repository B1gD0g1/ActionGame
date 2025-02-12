using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private PlayerControls playerControls;
    private AnimatorManager animatorManager;
    private PlayerMovement playerMovement;
    private ParkourControllerScript parkourController;

    public float moveAmount;

    //�洢��ҵ��ƶ�����ֵ
    [SerializeField] private Vector2 movementInput;
    //�洢��������ƶ�����ֵ
    [SerializeField] private Vector2 cameraInput;

    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;


    [Header("�ƶ���ť")]
    [SerializeField] private bool sprintInput;
    [SerializeField] private bool jumpInput;  //������Ծ����



    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
        parkourController = GetComponent<ParkourControllerScript>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // ���¼��������󶨵� Movement �� performed �¼�
            playerControls.PlayerMovement.Movement.performed += InputManager_OnMovementPerformed;
            playerControls.PlayerMovement.CameraMovement.performed += InputManager_OnCameraMovementPerformed;
            playerControls.PlayerActions.Sprint.performed += InputManager_OnSprintPerformed;
            playerControls.PlayerActions.Sprint.canceled += InputManager_OnSprintCanceled;
            playerControls.PlayerActions.Jump.performed += InputManager_OnJumpPerformed;
        }

        // ���� PlayerControls�������¼���ʼ����
        playerControls.Enable();
    }

    private void InputManager_OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        jumpInput = true;
    }

    private void InputManager_OnCameraMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        cameraInput = obj.ReadValue<Vector2>();
    }

    //������̼����ɿ�ʱ���¼�
    private void InputManager_OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        sprintInput = false;
    }
    //������̼�������ʱ���¼�
    private void InputManager_OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        sprintInput = true;
    }

    private void OnDisable()
    {
        // ���� PlayerControls��ֹͣ���������¼�
        playerControls.Disable();
    }

    //���ڴ���Movement�������¼�
    private void InputManager_OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        movementInput = obj.ReadValue<Vector2>();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintInput();
        HandleJumpInput();  // ������Ծ����
    }

    private void HandleMovementInput()
    {
        if (playerMovement.isClimbing)
        {
            // ֻ����ֱ���루ˮƽ����Ϊ 0��
            verticalInput = movementInput.y;
            horizontalInput = 0;  // ��ֹˮƽ����

            // ��������벻��
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }
        else
        {
            //����ƶ�����
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            //������ƶ�����
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }

        ////����ƶ�����
        //verticalInput = movementInput.y;
        //horizontalInput = movementInput.x;

        ////������ƶ�����
        //cameraInputX = cameraInput.x;
        //cameraInputY = cameraInput.y;

        //�����ƶ���
        //Mathf.Clamp01����������� [0, 1] ��Χ��
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animatorManager.UpdateAnimatorValues(0, moveAmount, playerMovement.isRunning);
    }

    //�жϳ��״̬
    private void HandleSprintInput()
    {
        if (sprintInput && moveAmount > 0.5f && playerMovement.isGrounded == true)
        {
            playerMovement.isRunning = true;
        }
        else
        {
            playerMovement.isRunning = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput && !playerMovement.isClimbing)
        {
            parkourController.TryStartParkour();  // ���Կ�ʼ�ܿ�/��������
            jumpInput = false;  // ������Ծ����
        }
    }

    // ��������״̬
    public void SetClimbingState(bool isClimbing)
    {
        playerMovement.isClimbing = isClimbing;
    }
}
