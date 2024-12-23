using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private PlayerControls playerControls;

    private AnimatorManager animatorManager;

    private float moveAmount;

    //存储玩家的移动输入值
    [SerializeField] private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;


    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // 将事件处理方法绑定到 Movement 的 performed 事件
            playerControls.PlayerMovement.Movement.performed += InputManager_OnMovementPerformed; ;
        }
        // 启用 PlayerControls，输入事件开始监听
        playerControls.Enable();
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
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //计算移动量
        //Mathf.Clamp01将结果限制在 [0, 1] 范围内
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
}
