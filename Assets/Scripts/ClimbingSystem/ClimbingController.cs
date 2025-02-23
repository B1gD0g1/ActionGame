using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    
    private PlayerMovement playerMovement;


    [Header("���ֶ������ѡ��")]
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

        //����ӽ�ɫ��������ķ���
        //Vector3 directionToLedge = ledge.position - transform.position;

        // �����xƽ�浽yƽ�����ת
        //if (Mathf.Approximately(Vector3.Angle(transform.forward, directionToLedge), 90f))
        //{
        //    // ȷ����ת�ǳ�����ȷ�ķ���
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

    //�������Idle����
    public void PlayIdleAnimationRandomly()
    {
        float randomFloat = Random.value;// ����0��1֮������float
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
