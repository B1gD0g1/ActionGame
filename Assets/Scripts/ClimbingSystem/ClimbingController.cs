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


    public IEnumerator JumpeToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    {
        var matchParams = new MatchTargetParams()
        {
            matchPosition = GetHandPosition(ledge),
            matchBodyPart = AvatarTarget.RightHand,
            matchStartTime = matchStartTime,
            matchTargetTime = matchTargetTime,
            matchPositionWeight = Vector3.one
        };

        Quaternion targetRotation = Quaternion.LookRotation(-ledge.forward);

        yield return playerMovement.DoAction(anim, matchParams, targetRotation, true);

        playerMovement.isHanging = true;
    }  

    private Vector3 GetHandPosition(Transform ledge)
    {
        return ledge.position - ledge.right * 0.07f + ledge.up * 0.05f - ledge.forward * 0.2f;
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
