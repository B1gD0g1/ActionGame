using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    private EnvironmentCheck environmentCheck;
    private InputManager inputManager;



    private void Awake()
    {
        environmentCheck = GetComponent<EnvironmentCheck>();
        inputManager = GetComponent<InputManager>();
    }


}
