using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerMovement playerMovement;
    private CameraManager cameraManager;


    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        //输入检测
        inputManager.HandleAllInputs();

        cameraManager.HandleAllCameraMovement();
    }

    private void FixedUpdate()
    {
        //实现物理移动
        playerMovement.HandleAllMovement();
    }
}
