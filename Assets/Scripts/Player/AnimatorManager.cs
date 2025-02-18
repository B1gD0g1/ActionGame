using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;

    private int horizontal;
    private int vertical;
    private int isClimbing;  // ����״̬�Ķ�������


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        isClimbing = Animator.StringToHash("IsClimbing");
    }

    //ͨ�������ˮƽ�ʹ�ֱ����ֵ�����¶�������
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0f && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1f;
        }
        else if (horizontalMovement < 0f && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1f;
        }
        else
        {
            snappedHorizontal = 0f;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0f && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1f;
        }
        else if (verticalMovement < 0f && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1f;
        }
        else
        {
            snappedVertical = 0f;
        }
        #endregion

        //���ų�̶����߼�
        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2f;
        }

        //���ö�������
        //if (true)
        //{

        //}
        animator.SetFloat(horizontal, snappedHorizontal, 0.2f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.2f, Time.deltaTime);
    }

    // ��������������״̬
    public void SetClimbingState(bool isClimbingState)
    {
        animator.SetBool(isClimbing, isClimbingState);
    }

    public void SetFloatAnimator(string id, float value)
    {
        animator.SetFloat(id, value);
    }

    public void SetFloatAnimator(string id, float value, float dampTime, float deltaTime)
    {
        animator.SetFloat(id, value, dampTime, deltaTime);
    }

    public void SetBoolAnimator(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
