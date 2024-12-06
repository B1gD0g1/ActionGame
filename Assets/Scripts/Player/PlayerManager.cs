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
        //������
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        //ʵ�������ƶ�
        playerMovement.HandleAllMovement();
    }
}
