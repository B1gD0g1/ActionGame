using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private PlayerControls playerControls;
    private AnimatorManager animatorManager;
    private PlayerMovement playerMovement;
    private ParkourControllerScript parkourController;
    private EnvironmentCheck environmentCheck;
    private ClimbingController climbingController;

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
    [SerializeField] private bool LandingInput;
    [SerializeField] private bool shootInput;
    [SerializeField] private bool scopeInput;
    [SerializeField] private bool reloadInput;
    [SerializeField] private bool changeWeaponInput;
    [SerializeField] private bool pauseInput;


    [Header("�����ƶ�")]
    private ClimbPoint currentPoint;



    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
        parkourController = GetComponent<ParkourControllerScript>();
        environmentCheck = GetComponent<EnvironmentCheck>();
        climbingController = GetComponent<ClimbingController>();
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
            playerControls.PlayerActions.Jump.canceled += InputManager_OnJumpCanceled;
            playerControls.PlayerActions.JumpFromHang.performed += InputManager_OnJumpFromHangPerformed;
            playerControls.PlayerActions.JumpFromHang.canceled += InputManager_OnJumpFromHangCanceled;

            playerControls.PlayerActions.Shoot.performed += i => shootInput = true;
            playerControls.PlayerActions.Shoot.canceled += i => shootInput = false;
            playerControls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerControls.PlayerActions.Scope.canceled += i => scopeInput = false;
            playerControls.PlayerActions.Reload.performed += i => reloadInput = true;
            playerControls.PlayerActions.Reload.canceled += i => reloadInput = false;
            playerControls.PlayerActions.Change.performed += i => changeWeaponInput = true;
            playerControls.PlayerActions.Pause.performed += i => pauseInput = true;
        }

        // ���� PlayerControls�������¼���ʼ����
        playerControls.Enable();
    }

    private void InputManager_OnJumpFromHangCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        LandingInput = false;
    }

    private void InputManager_OnJumpFromHangPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        LandingInput = true;
    }

    private void InputManager_OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        jumpInput = false;
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
        HandleClimbInput();
        StartCoroutine(HandleChangeRifleInput());
        HandlePauseInput();
    }

    private void HandleMovementInput()
    {
        if (playerMovement.isJumping)
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

        //�����ƶ���
        //Mathf.Clamp01����������� [0, 1] ��Χ��
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        //animatorManager.UpdateAnimatorValues(0f, moveAmount, playerMovement.isRunning);

        if (playerMovement.IsOnLedge)
        {
            animatorManager.UpdateAnimatorValues(0f, 0f, false);
        }
        else
        {
            animatorManager.UpdateAnimatorValues(0f, moveAmount, playerMovement.isRunning);
        }
    }

    //�жϳ��״̬
    private void HandleSprintInput()
    {
        if (sprintInput && moveAmount > 0.5f
            && (playerMovement.isGrounded == true || playerMovement.isOnObstacle == true)
            && playerMovement.IsOnLedge == false)
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
        var hitData = environmentCheck.CheckObstacle();

        if (jumpInput && !playerMovement.isJumping && !playerMovement.IsHanging)
        {
            parkourController.TryStartParkour(hitData);  // ���Կ�ʼ�ܿ�/��������
        }

        if (playerMovement.IsOnLedge && playerMovement.InAction == false
            && hitData.hitFound == false)
        {
            parkourController.TryStartJumpDown(hitData);  // ���Կ�ʼ�ܿ�/��������
        }

        //if (playerMovement.playerOnLedge)
        //{
        //    parkourController.TryStartParkour();
        //}
    }

    private void HandleClimbInput()
    {
        if (!playerMovement.IsHanging)
        {
            if (jumpInput && !playerMovement.InAction)
            {
                if (environmentCheck.ClimbeLedgeCheck(environmentCheck.transform.forward, out RaycastHit climbHitInfo))
                {
                    //currentPoint = climbHitInfo.transform.GetComponent<ClimbPoint>();
                    currentPoint = climbingController.GetNearestClimbPoint(climbHitInfo.transform, climbHitInfo.point);

                    playerMovement.SetControl(false);
                    UnityEngine.Debug.Log("���������ҵ�");
                    StartCoroutine(climbingController.JumpeToLedge("IdleToHang",
                        currentPoint.transform, 0.45f, 0.76f));
                }
            }

            if (LandingInput && !playerMovement.InAction)
            {
                if (environmentCheck.DropLedgeCheck(out RaycastHit ledgeHit))
                {
                    UnityEngine.Debug.Log("�ҵ������㣡����");

                    currentPoint = climbingController.GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerMovement.SetControl(false);
                    StartCoroutine(climbingController.JumpeToLedge("DropToHang",
                        currentPoint.transform, 0.17f, 0.66f, handOffset: new Vector3(0.2f, 0.03f, 0.02f)));
                }
            }

        }
        else
        {
            //����������½
            if (LandingInput && !playerMovement.InAction)
            {
                StartCoroutine(climbingController.JumpFromHang());
                return;
            }

            if (playerMovement.InAction || movementInput == Vector2.zero) return;

            //��������ߵ����ݶ�
            if (currentPoint.MountPoint && movementInput.y == 1)
            {
                StartCoroutine(climbingController.MountFromHang());
                return;
            }

            //�ӵ�ǰ������������һ��������
            var neighbour = currentPoint.GetNeighbour(movementInput);

            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && jumpInput)
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.y == 1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopUp", currentPoint.transform, 0.34f, 0.65f, handOffset: new Vector3(0.33f, 0.02f, -0.15f)));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f, handOffset: new Vector3(0.33f, 0.02f, -0.15f)));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f, handOffset: new Vector3(0.33f, 0.01f, -0.09f)));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f, handOffset: new Vector3(0.42f, 0.01f, -0.09f)));
            }
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.x == 1)
                    StartCoroutine(climbingController.JumpeToLedge("ShimmyRight", currentPoint.transform, 0.15f, 0.58f, handOffset: new Vector3(0.42f, 0.015f, -0.05f)));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(climbingController.JumpeToLedge("ShimmyLeft", currentPoint.transform, 0.15f, 0.58f, AvatarTarget.LeftHand, handOffset: new Vector3(0.32f, 0.015f, -0.05f)));
            }
        }
    }

    private IEnumerator HandleChangeRifleInput()
    {
        yield return new WaitForSeconds(0.2f);
        if (changeWeaponInput)
        {
            changeWeaponInput = false;
        }
    }
    private void HandlePauseInput()
    {
        if (pauseInput)
        {
            pauseInput = false;
        }
    }

    // ��������״̬
    public void SetJumpingState(bool isJumping)
    {
        playerMovement.isJumping = isJumping;
    }

    //��¶jumpInput
    public bool GetJumpInput()
    {
        return jumpInput;
    }

    public bool GetSprintInput()
    {
        return sprintInput;
    }

    public bool GetChangeRifleInput()
    {
        return changeWeaponInput;
    }

    public bool GetShootInput()
    {
        return shootInput;
    }

    public bool GetReloadInput()
    {
        return reloadInput;
    }
}
