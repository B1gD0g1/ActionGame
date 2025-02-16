using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourControllerScript : MonoBehaviour
{

    private EnvironmentCheck environmentCheck;
    private PlayerMovement playerMovement;
    private InputManager inputManager;

    public Animator animator;


    private bool playerInAction;


    [Header("�ܿᶯ������")]
    [SerializeField] private List<NewParkourAction> parkourActions;

    [Header("���¶���")]
    [SerializeField] private NewParkourAction jumpDownParkourAction;

    private void Awake()
    {
        environmentCheck = GetComponent<EnvironmentCheck>();
        playerMovement = GetComponent<PlayerMovement>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
      
    } 

    public void TryStartParkour(ObstacleInfo hitData)
    {
        if (playerInAction) return;

        //var hitData = environmentCheck.CheckObstacle();

        if (hitData.hitFound)
        {
            //Debug.Log("��⵽�ϰ�: " + hitData.hitInfo.transform.name);

            foreach (var action in parkourActions)
            {
                if (action.CheckIfAvailable(hitData, transform))
                {
                    StartCoroutine(PerformParkourAction(action));
                    break;
                }
            }
        }

        if (playerMovement.IsOnLedge && playerInAction == false 
            && hitData.hitFound == false && inputManager.GetJumpInput() == true)
        {
            if (playerMovement.LedgeInfo.angle <= 50)
            {
                playerMovement.IsOnLedge = false;
                StartCoroutine(PerformParkourAction(jumpDownParkourAction));
            }
        }
    }

    private IEnumerator PerformParkourAction(NewParkourAction action)
    {
        playerInAction = true;

        playerMovement.SetControl(false);

        // ִ����������
        animator.CrossFade(action.AnimationName, 0.2f);
        yield return null;

        // ֪ͨ�����������ʼ����
        inputManager.SetClimbingState(true);

        var animatorState = animator.GetNextAnimatorStateInfo(0);
        if (animatorState.IsName(action.AnimationName) == false)
        {
            Debug.LogError("�������Ʋ�ƥ�䣡");
        }

        //yield return new WaitForSeconds(animatorState.length);

        float timer = 0f;
        while (timer <= animatorState.length)
        {
            timer += Time.deltaTime;

            if (action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    action.TargetRotation,
                    playerMovement.RotationSpeed * Time.deltaTime);
            }

            if (action.EnableTargetMatching)
            {
                MatchTarget(action);
            }

            if (animator.IsInTransition(0) && timer > 0.5f)
            {
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay);

        // ��������󣬻ָ���������ģʽ
        inputManager.SetClimbingState(false);

        playerMovement.SetControl(true);

        playerInAction = false;
    }

    private void MatchTarget(NewParkourAction action)
    {
        if (animator.isMatchingTarget)
        {
            return;
        }

        animator.MatchTarget(action.MatchPosition, 
            transform.rotation, 
            action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPositionWeight, 0), 
            action.MatchStartTime, 
            action.MatchTargetTime);
    }
}
