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
        //������
        inputManager.HandleAllInputs();

        cameraManager.HandleAllCameraMovement();
    }

    private void FixedUpdate()
    {
        //ʵ�������ƶ�
        playerMovement.HandleAllMovement();
    }
}
