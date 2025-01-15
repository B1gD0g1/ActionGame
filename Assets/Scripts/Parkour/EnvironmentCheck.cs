using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    //�������ߵ���ʼλ��
    [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
    //���ߵĳ���
    [SerializeField] private float rayLength = 0.9f;
    [SerializeField] private LayerMask obstacleLayer;



    public void CheckObstacle()
    {
        //�������
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

        //��ʾ����
        Debug.DrawRay(rayOrigin, transform.forward * rayLength, rayColor);
    }
}
