using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourControllerScript : MonoBehaviour
{

    [SerializeField] private EnvironmentCheck environmentCheck;


    private void Update()
    {
        var hitData = environmentCheck.CheckObstacle();


        if (hitData.hitFound)
        {
            Debug.Log("ºÏ≤‚µΩ’œ∞≠" + hitData.hitInfo.transform.name);
        }

    }

}
