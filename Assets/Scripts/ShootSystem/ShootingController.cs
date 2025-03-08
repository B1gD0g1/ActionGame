using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private Animator animator;


    [SerializeField] private Transform firePoint;

    [Header("武器数值")]
    [SerializeField] private float fireRate;//射速
    [SerializeField] private int magzineCapacity;//弹匣容量
    [SerializeField] private int maxAmmo;
    [SerializeField] private float fireRange;//射击长度
    [SerializeField] private float giveDamageOf;
    [SerializeField] private float reloadTime;

    private float nextFireTime;
    private int currentMagzine;
    private int currentAmmo;

    [SerializeField] private bool isReloading;

    [Header("声音")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootingSoundClip;
    [SerializeField] private AudioClip reloadingSoundClip;

    [Header("武器特效")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bloodEffect;


    private void Start()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        currentMagzine = magzineCapacity;
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        if (inputManager.GetShootInput() && Time.time >= nextFireTime && currentMagzine > 0
            && !isReloading)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if (inputManager.GetReloadInput() && currentMagzine < magzineCapacity && currentAmmo > 0
            && !isReloading)
        {
            StartCoroutine(Reload());
            animator.SetTrigger("Reloading");
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
        {
            Debug.Log("击中" + hit.transform.name);

            //警卫
            Guard guard = hit.transform.GetComponent<Guard>();

            if (guard != null)
            {
                guard.CharacterHitDamage(giveDamageOf);

                //血液效果
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //如果有守卫看见尸体，则提示守卫
            }

            //NPC
            WalkCharacterAI characterNPC = hit.transform.GetComponent<WalkCharacterAI>();

            if (characterNPC != null)
            {
                characterNPC.CharacterHitDamage(giveDamageOf);

                //血液效果
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //如果有守卫看见尸体，则提示守卫
            }

            //Boss
            Boss boss = hit.transform.GetComponent<Boss>();

            if (boss != null)
            {
                boss.CharacterHitDamage(giveDamageOf);

                //血液效果
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //如果有守卫看见尸体，则提示守卫
            }
        }

        currentMagzine--;

        //播放射击声音
        audioSource.PlayOneShot(shootingSoundClip);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        //换弹声音
        yield return new WaitForSeconds(0.7f);//延迟，同步动画
        audioSource.PlayOneShot(reloadingSoundClip);

        int ammoToReload = Mathf.Min(magzineCapacity - currentMagzine, currentAmmo);

        //等待动画完成
        yield return new WaitForSeconds(reloadTime);

        currentMagzine += ammoToReload;
        currentAmmo -= ammoToReload;

        if (currentAmmo < maxAmmo - magzineCapacity)
        {
            maxAmmo = currentAmmo + magzineCapacity;
        }

        isReloading = false;
    }
}
