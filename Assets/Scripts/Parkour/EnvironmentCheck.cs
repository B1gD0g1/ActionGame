using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    //调整射线的起始位置
    [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
    //射线的长度
    [SerializeField] private float rayLength = 0.9f;
    [SerializeField] private LayerMask obstacleLayer;



    public void CheckObstacle()
    {
        //射线起点
        var rayOrigin = transform.position + rayOffset;
        bool hitFound = Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hitInfo, rayLength, obstacleLayer);

        UnityEngine.Color rayColor;
        if (hitFound)
        {
            rayColor = UnityEngine.Color.red;
        }
        else
        {
            rayColor = UnityEngine.Color.green;
        }

        //显示射线
        Debug.DrawRay(rayOrigin, transform.forward * rayLength, rayColor);
    }
}
