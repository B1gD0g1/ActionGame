using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator animator;


    [SerializeField] private float health;
    private bool isDead = false;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip endSound;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void CharacterHitDamage(float takeDamage)
    {
        if (isDead)
            return;

        health -= takeDamage;

        if (health <= 0)
        {
            animator.SetTrigger("Die");
            CharacterDie();
        }
    }

    private void CharacterDie()
    {
        //½áÊøÉùÒô ending voice sound
        if (audioSource != null && endSound != null)
        {
            audioSource.clip = endSound;
            audioSource.Play();
        }
    }
}
