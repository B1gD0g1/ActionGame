using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private PlayerControls playerControls;
    private AnimatorManager animatorManager;
    private PlayerMovement playerMovement;

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

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
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
        }
        // 启用 PlayerControls，输入事件开始监听
        playerControls.Enable();
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
    }

    private void HandleMovementInput()
    {
        //玩家移动输入
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //摄像机移动输入
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        //计算移动量
        //Mathf.Clamp01将结果限制在 [0, 1] 范围内
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animatorManager.UpdateAnimatorValues(0, moveAmount, playerMovement.isRunning);
    }

    //判断冲刺状态
    private void HandleSprintInput()
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerMovement.isRunning = true;
        }
        else
        {
            playerMovement.isRunning = false;
        }
    }
}
