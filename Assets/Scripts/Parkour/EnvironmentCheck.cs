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
    [SerializeField] private float heightRayLength = 6f;
    [SerializeField] private LayerMask obstacleLayer;



    public ObstacleInfo CheckObstacle()
    {

        var hitData = new ObstacleInfo();

        //前方射线起点
        var rayOrigin = transform.position + rayOffset;
        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, obstacleLayer);

        // 调试前方射线信息
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
        //显示射线
        Debug.DrawRay(rayOrigin, transform.forward * rayLength, rayColor);


        //高度射线检测
        if (hitData.hitFound)
        {
            var heightRayOrigin = hitData.hitInfo.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down, out hitData.heightHitInfo, heightRayLength, obstacleLayer);

            // 调试高度射线信息
            Debug.Log($"Height Ray - Origin: {heightRayOrigin}, Direction: Vector3.down, Hit Found: {hitData.heightHitFound}");

            //显示射线
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
