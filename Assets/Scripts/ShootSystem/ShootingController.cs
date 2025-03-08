using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private Animator animator;


    [SerializeField] private Transform firePoint;

    [Header("������ֵ")]
    [SerializeField] private float fireRate;//����
    [SerializeField] private int magzineCapacity;//��ϻ����
    [SerializeField] private int maxAmmo;
    [SerializeField] private float fireRange;//�������
    [SerializeField] private float giveDamageOf;
    [SerializeField] private float reloadTime;

    private float nextFireTime;
    private int currentMagzine;
    private int currentAmmo;

    [SerializeField] private bool isReloading;

    [Header("����")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootingSoundClip;
    [SerializeField] private AudioClip reloadingSoundClip;

    [Header("������Ч")]
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
            Debug.Log("����" + hit.transform.name);

            //����
            Guard guard = hit.transform.GetComponent<Guard>();

            if (guard != null)
            {
                guard.CharacterHitDamage(giveDamageOf);

                //ѪҺЧ��
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //�������������ʬ�壬����ʾ����
            }

            //NPC
            WalkCharacterAI characterNPC = hit.transform.GetComponent<WalkCharacterAI>();

            if (characterNPC != null)
            {
                characterNPC.CharacterHitDamage(giveDamageOf);

                //ѪҺЧ��
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //�������������ʬ�壬����ʾ����
            }

            //Boss
            Boss boss = hit.transform.GetComponent<Boss>();

            if (boss != null)
            {
                boss.CharacterHitDamage(giveDamageOf);

                //ѪҺЧ��
                GameObject bloodEfectGo = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEfectGo, 0.5f);

                //�������������ʬ�壬����ʾ����
            }
        }

        currentMagzine--;

        //�����������
        audioSource.PlayOneShot(shootingSoundClip);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        //��������
        yield return new WaitForSeconds(0.7f);//�ӳ٣�ͬ������
        audioSource.PlayOneShot(reloadingSoundClip);

        int ammoToReload = Mathf.Min(magzineCapacity - currentMagzine, currentAmmo);

        //�ȴ��������
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
