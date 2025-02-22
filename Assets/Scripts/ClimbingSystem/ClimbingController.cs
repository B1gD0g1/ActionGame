using System.Collections;
using System.Collections.Generic;
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

        Quaternion targetRotation = Quaternion.LookRotation(ledge.right);

        yield return playerMovement.DoAction(anim, matchParams, targetRotation, true);

        playerMovement.IsHanging = true;
    }  

    private Vector3 GetHandPosition(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        var offsetValue = (handOffset != null) ? handOffset.Value : new Vector3(0.07f, 0.05f, 0.38f);

        var hDir = hand == AvatarTarget.RightHand ? ledge.forward : -ledge.forward;

        return ledge.position - ledge.right * offsetValue.x + ledge.up * offsetValue.y - hDir * offsetValue.z;
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
