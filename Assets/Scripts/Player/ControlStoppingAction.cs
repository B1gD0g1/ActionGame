using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStoppingAction : StateMachineBehaviour
{
    private PlayerMovement playerMovement;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerMovement == null)
        {
            playerMovement = animator.GetComponent<PlayerMovement>();
        }

        playerMovement.HasControl = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerMovement.HasControl = true;
    }
}
