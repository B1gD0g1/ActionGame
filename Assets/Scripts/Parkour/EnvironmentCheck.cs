using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    //�������ߵ���ʼλ��
    [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
    //���ߵĳ���
    [SerializeField] private float rayLength = 0.9f;
    [SerializeField] private float heightRayLength = 6f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask surfaceLayer;


    [Header("������¸߶� Check Ledge")]
    [SerializeField] private float ledgeRayLength = 11f;
    [SerializeField] private float ledgeRayHeightThresHold = 0.76f;



    public ObstacleInfo CheckObstacle()
    {

        var hitData = new ObstacleInfo();

        //ǰ���������
        var rayOrigin = transform.position + rayOffset;
        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, obstacleLayer);

        // ����ǰ��������Ϣ
        //Debug.Log($"Forward Ray - Origin: {rayOrigin}, Direction: {transform.forward}, Hit Found: {hitData.hitFound}");

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
        //Debug.DrawRay(rayOrigin, transform.forward * rayLength, rayColor);


        //�߶����߼��
        if (hitData.hitFound)
        {
            var heightRayOrigin = hitData.hitInfo.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down, out hitData.heightHitInfo, heightRayLength, obstacleLayer);

            // ���Ը߶�������Ϣ
            //Debug.Log($"Height Ray - Origin: {heightRayOrigin}, Direction: Vector3.down, Hit Found: {hitData.heightHitFound}");

            //��ʾ����
            //Debug.DrawRay(heightRayOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? UnityEngine.Color.blue : UnityEngine.Color.green);
        }

        return hitData;
    }

    public bool CheckLedge(Vector3 moveDirection, out LedgeInfo ledgeInfo)
    {
        ledgeInfo = new LedgeInfo();

        if (moveDirection == Vector3.zero)
        {
            return false;
        }

        float ledgeOriginOffet = 0.5f;
        var ledgeOrigin = transform.position + moveDirection * ledgeOriginOffet + Vector3.up;

        if (PhysicsUtillity.ThreeRaycasts(ledgeOrigin, Vector3.down, 0.25f, transform,
            out List<RaycastHit> hitDatas, ledgeRayLength, obstacleLayer, true))
        {
            //Debug.DrawRay(ledgeOrigin, Vector3.down * ledgeRayLength, UnityEngine.Color.yellow);

            var validHits = hitDatas.Where(h =>
            transform.position.y - h.point.y > ledgeRayHeightThresHold).ToList();

            if (validHits.Count > 0)
            {
                //var surfaceRaycastOrigin = transform.position + moveDirection - new Vector3(0, 0.1f, 0);
                var surfaceRaycastOrigin = validHits[0].point;
                surfaceRaycastOrigin.y = transform.position.y - 0.1f;

                if (Physics.Raycast(surfaceRaycastOrigin, transform.position - surfaceRaycastOrigin,
                    out RaycastHit surfaceHit, 2f, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRaycastOrigin, transform.position, UnityEngine.Color.cyan);

                    float ledgeHeight = transform.position.y - validHits[0].point.y;

                    ledgeInfo.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeInfo.height = ledgeHeight;
                    ledgeInfo.surfaceHit = surfaceHit;

                    return true;
                }
            }
        }
        return false;
    }
}

public struct ObstacleInfo
{
    public bool hitFound;
    public RaycastHit hitInfo;
    public bool heightHitFound;
    public RaycastHit heightHitInfo;
}

public struct LedgeInfo
{
    public float angle;
    public float height;
    public RaycastHit surfaceHit;
}
