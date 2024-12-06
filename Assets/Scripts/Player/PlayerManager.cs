using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerMovement playerMovement;


    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //输入检测
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        //实现物理移动
        playerMovement.HandleAllMovement();
    }
}
