using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    
    private PlayerMovement playerMovement;


    [Header("两种动画随机选择")]
    public bool hangingIdleRandom;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }


    public IEnumerator JumpeToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime,
        AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null)
    {
        var matchParams = new MatchTargetParams()
        {
            matchPosition = GetHandPosition(ledge, hand, handOffset),
            matchBodyPart = hand,
            matchStartTime = matchStartTime,
            matchTargetTime = matchTargetTime,
            matchPositionWeight = Vector3.one
        };
        
        Quaternion targetRotation = Quaternion.LookRotation(-ledge.forward);

        //计算从角色到攀爬点的方向
        //Vector3 directionToLedge = ledge.position - transform.position;

        // 计算从x平面到y平面的旋转
        //if (Mathf.Approximately(Vector3.Angle(transform.forward, directionToLedge), 90f))
        //{
        //    // 确保旋转是朝向正确的方向
        //    targetRotation = Quaternion.LookRotation(-directionToLedge);
        //    //UnityEngine.Debug.Log("Rotating across planes: " + targetRotation);
        //}
        //else
        //{
        //    targetRotation = Quaternion.LookRotation(-ledge.forward);
        //    //UnityEngine.Debug.Log("Normal rotation: " + targetRotation);
        //}
        
        yield return playerMovement.DoAction(anim, matchParams, targetRotation, true);

        playerMovement.IsHanging = true;
    }  

    private Vector3 GetHandPosition(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        //var offsetValue = (handOffset != null) ? handOffset.Value : new Vector3(0.07f, 0.05f, 0.38f);
        var offsetValue = (handOffset != null) ? handOffset.Value : new Vector3(0.2f, 0.03f, 0.08f);

        var hDir = hand == AvatarTarget.RightHand ? ledge.right : -ledge.right;

        return ledge.position - hDir * offsetValue.x + ledge.up * offsetValue.y - ledge.forward * offsetValue.z;
    } 

    public IEnumerator JumpFromHang()
    {
        playerMovement.IsHanging = false;
        yield return playerMovement.DoAction("JumpFromHang");

        playerMovement.ResetTargetRotation();
        playerMovement.SetControl(true);
    }

    public IEnumerator MountFromHang()
    {
        playerMovement.IsHanging = false;
        yield return playerMovement.DoAction("MountFromHang");

        //启用角色控制器，防止播放站立动画时穿模
        playerMovement.EnableCharacterController(true);

        //等待站立动画播放完才能控制移动
        yield return new WaitForSeconds(0.5f);

        playerMovement.ResetTargetRotation();
        playerMovement.SetControl(true);
    }

    public ClimbPoint GetNearestClimbPoint(Transform ledge, Vector3 hitPoint)
    {
        var points = ledge.GetComponentsInChildren<ClimbPoint>();

        ClimbPoint nearestPoint = null;
        //存储最近的点
        float nearestPointDistance = Mathf.Infinity;

        foreach (var point in points)
        {
            var distance = Vector3.Distance(point.transform.position, hitPoint);

            if (distance < nearestPointDistance)
            {
                nearestPoint = point;
                nearestPointDistance = distance;
            }
        }

        return nearestPoint;
    }

    //随机播放Idle动画
    public void PlayIdleAnimationRandomly()
    {
        float randomFloat = Random.value;// 生成0到1之间的随机float
        if (randomFloat > 0.5f)
        {
            hangingIdleRandom = true;
        }
        else
        {
            hangingIdleRandom = false;
        }
    }
}
