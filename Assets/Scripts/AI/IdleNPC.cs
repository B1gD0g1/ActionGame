using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleNPC : MonoBehaviour
{
    private Animator animator;


    private int currentIndex = 0;
    private int totalAnimations = 4;


    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();


        InvokeRepeating(nameof(SwitchAnimation), 10f, 10f); // ÿ 10 ���л�һ��
    }

    private void SwitchAnimation()
    {
        currentIndex = (currentIndex + 1) % totalAnimations;
        animator.SetInteger("Index", currentIndex); // ��Animator�д���"AnimationIndex"����
    }
}
