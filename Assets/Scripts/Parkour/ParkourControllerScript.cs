using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParkourControllerScript : MonoBehaviour
{

    private EnvironmentCheck environmentCheck;
    private PlayerMovement playerMovement;
    private InputManager inputManager;
    private PlayerControls playerControls;

    public Animator animator;


    [Header("跑酷动作区域")]
    [SerializeField] private List<NewParkourAction> parkourActions;

    [Header("跳下动作")]
    [SerializeField] private NewParkourAction jumpDownParkourAction;

    [Header("自动跳下高度")]
    [SerializeField] private float autoDropHeightLimit;

    private void Awake()
    {
        environmentCheck = GetComponent<EnvironmentCheck>();
        playerMovement = GetComponent<PlayerMovement>();
        inputManager = GetComponent<InputManager>();

        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    private void Update()
    {

    }

    public void TryStartParkour(ObstacleInfo hitData)
    {
        if (playerMovement.InAction) return;

        //var hitData = environmentCheck.CheckObstacle();

        if (hitData.hitFound)
        {
            //Debug.Log("检测到障碍: " + hitData.hitInfo.transform.name);

            foreach (var action in parkourActions)
            {
                if (action.CheckIfAvailable(hitData, transform))
                {
                    StartCoroutine(PerformParkourAction(action));
                    break;
                }
            }
        }
    }

    public void TryStartJumpDown(ObstacleInfo hitData)
    {
        bool shouldJump = true;
        if (playerMovement.LedgeInfo.height > autoDropHeightLimit
            && playerControls.PlayerActions.Jump.triggered == false)
        {
            shouldJump = false;
        }

        if (shouldJump && playerMovement.LedgeInfo.angle <= 50)
        {
            playerMovement.IsOnLedge = false;
            StartCoroutine(PerformParkourAction(jumpDownParkourAction));
        }
    }

    private IEnumerator PerformParkourAction(NewParkourAction action)
    {
        playerMovement.SetControl(false);

        MatchTargetParams matchTargetParams = null;
        if (action.EnableTargetMatching)
        {
            matchTargetParams = new MatchTargetParams()
            {
                matchPosition = action.MatchPosition,
                matchBodyPart = action.MatchBodyPart,
                matchPositionWeight = action.MatchPositionWeight,
                matchStartTime = action.MatchStartTime,
                matchTargetTime = action.MatchTargetTime,
            };
        }

        yield return playerMovement.DoAction(action.AnimationName, matchTargetParams,
            action.TargetRotation, action.RotateToObstacle, action.PostActionDelay);

        playerMovement.SetControl(true);

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

    public bool GetPlayerInAction()
    {
        return playerMovement.InAction;
    }
}
