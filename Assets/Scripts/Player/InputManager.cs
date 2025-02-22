using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    //存储玩家的移动输入值
    [SerializeField] private Vector2 movementInput;
    //存储摄像机的移动输入值
    [SerializeField] private Vector2 cameraInput;

    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;


    [Header("移动按钮")]
    [SerializeField] private bool sprintInput;
    [SerializeField] private bool jumpInput;  //控制跳跃输入

    [Header("攀爬移动")]
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

            // 将事件处理方法绑定到 Movement 的 performed 事件
            playerControls.PlayerMovement.Movement.performed += InputManager_OnMovementPerformed;
            playerControls.PlayerMovement.CameraMovement.performed += InputManager_OnCameraMovementPerformed;
            playerControls.PlayerActions.Sprint.performed += InputManager_OnSprintPerformed;
            playerControls.PlayerActions.Sprint.canceled += InputManager_OnSprintCanceled;
            playerControls.PlayerActions.Jump.performed += InputManager_OnJumpPerformed;
            playerControls.PlayerActions.Jump.canceled += InputManager_OnJumpCanceled;
        }

        // 启用 PlayerControls，输入事件开始监听
        playerControls.Enable();
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

    //监听冲刺键被松开时的事件
    private void InputManager_OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        sprintInput = false;
    }
    //监听冲刺键被按下时的事件
    private void InputManager_OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        sprintInput = true;
    }

    private void OnDisable()
    {
        // 禁用 PlayerControls，停止监听输入事件
        playerControls.Disable();
    }

    //用于处理Movement的输入事件
    private void InputManager_OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        movementInput = obj.ReadValue<Vector2>();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintInput();
        HandleJumpInput();  // 处理跳跃输入
        HandleClimbInput();
    }

    private void HandleMovementInput()
    {
        if (playerMovement.isJumping)
        {
            // 只允许垂直输入（水平输入为 0）
            verticalInput = movementInput.y;
            horizontalInput = 0;  // 禁止水平输入

            // 摄像机输入不变
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }
        else
        {
            //玩家移动输入
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            //摄像机移动输入
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }

        ////玩家移动输入
        //verticalInput = movementInput.y;
        //horizontalInput = movementInput.x;

        ////摄像机移动输入
        //cameraInputX = cameraInput.x;
        //cameraInputY = cameraInput.y;

        //计算移动量
        //Mathf.Clamp01将结果限制在 [0, 1] 范围内
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

    //判断冲刺状态
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
            parkourController.TryStartParkour(hitData);  // 尝试开始跑酷/攀爬动作
        }

        if (playerMovement.IsOnLedge && playerMovement.InAction == false
            && hitData.hitFound == false)
        {
            parkourController.TryStartJumpDown(hitData);  // 尝试开始跑酷/攀爬动作
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
                if (environmentCheck.ClimbeLedgeCheck(transform.forward, out RaycastHit climbHitInfo))
                {
                    currentPoint = climbHitInfo.transform.GetComponent<ClimbPoint>();

                    playerMovement.SetControl(false);
                    UnityEngine.Debug.Log("攀爬点已找到");
                    StartCoroutine(climbingController.JumpeToLedge("IdleToHang",
                        climbHitInfo.transform, 0.45f, 0.76f));
                }
            }
        }
        else
        {
            //从这个攀爬点跳到另一个攀爬点
            if (playerMovement.InAction || movementInput == Vector2.zero) return;

            var neighbour = currentPoint.GetNeighbour(movementInput);

            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && jumpInput)
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.y == 1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopUp", currentPoint.transform, 0.34f, 0.65f, handOffset: new Vector3(0.18f, 0.02f, 0.3f)));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f, handOffset: new Vector3(0.18f, 0.02f, 0.3f)));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(climbingController.JumpeToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f));
            }
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.x == 1)
                    StartCoroutine(climbingController.JumpeToLedge("ShimmyRight", currentPoint.transform, 0.15f, 0.58f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(climbingController.JumpeToLedge("ShimmyLeft", currentPoint.transform, 0.15f, 0.58f, AvatarTarget.LeftHand));
            }
        }
    }

    // 设置攀爬状态
    public void SetJumpingState(bool isJumping)
    {
        playerMovement.isJumping = isJumping;
    }

    //暴露jumpInput
    public bool GetJumpInput()
    {
        return jumpInput;
    }

    public bool GetSprintInput()
    {
        return sprintInput;
    }
}
