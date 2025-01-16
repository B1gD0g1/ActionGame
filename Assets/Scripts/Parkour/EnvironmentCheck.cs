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
    [SerializeField] private float heightRayLength = 6f;
    [SerializeField] private LayerMask obstacleLayer;



    public ObstacleInfo CheckObstacle()
    {

        var hitData = new ObstacleInfo();

        //ǰ���������
        var rayOrigin = transform.position + rayOffset;
        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, obstacleLayer);

        // ����ǰ��������Ϣ
        Debug.Log($"Forward Ray - Origin: {rayOrigin}, Direction: {transform.forward}, Hit Found: {hitData.hitFound}");

        UnityEngine.Color rayColor;
        if (hitData.hitFound)
        {
            rayColor = UnityEngine.Color.red;
        }
        else
        {
            rayColor = UnityEngine.Color.green;
        }
        //��ʾ����
        Debug.DrawRay(rayOrigin, transform.forward * rayLength, rayColor);


        //�߶����߼��
        if (hitData.hitFound)
        {
            var heightRayOrigin = hitData.hitInfo.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down, out hitData.heightHitInfo, heightRayLength, obstacleLayer);

            // ���Ը߶�������Ϣ
            Debug.Log($"Height Ray - Origin: {heightRayOrigin}, Direction: Vector3.down, Hit Found: {hitData.heightHitFound}");

            //��ʾ����
            Debug.DrawRay(heightRayOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? UnityEngine.Color.blue : UnityEngine.Color.green);
        }

        return hitData;
    }
}

public struct ObstacleInfo
{
    public bool hitFound;
    public RaycastHit hitInfo;
    public bool heightHitFound;
    public RaycastHit heightHitInfo;

}
