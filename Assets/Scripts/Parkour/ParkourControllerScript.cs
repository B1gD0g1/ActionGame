using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourControllerScript : MonoBehaviour
{

    [SerializeField] private EnvironmentCheck environmentCheck;


    private void Update()
    {
        environmentCheck.CheckObstacle();
    }

}
