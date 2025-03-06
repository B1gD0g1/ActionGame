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


        InvokeRepeating(nameof(SwitchAnimation), 10f, 10f); // 每 10 秒切换一次
    }

    private void SwitchAnimation()
    {
        currentIndex = (currentIndex + 1) % totalAnimations;
        animator.SetInteger("Index", currentIndex); // 在Animator中创建"AnimationIndex"参数
    }
}
