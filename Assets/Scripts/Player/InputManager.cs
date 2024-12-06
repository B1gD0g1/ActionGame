using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private PlayerControls playerControls;

    //�洢��ҵ��ƶ�����ֵ
    [SerializeField] private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;



    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // ���¼��������󶨵� Movement �� performed �¼�
            playerControls.PlayerMovement.Movement.performed += InputManager_OnMovementPerformed; ;
        }
        // ���� PlayerControls�������¼���ʼ����
        playerControls.Enable();
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
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }
}
