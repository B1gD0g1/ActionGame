using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private Animator animator;


    [SerializeField] private Transform firePoint;

    [Header("������ֵ")]
    [SerializeField] private float fireRate = 1f;//����
    [SerializeField] private int magzineCapacity = 30;//��ϻ����
    [SerializeField] private int maxAmmo = 300;
    [SerializeField] private float fireRange = 100f;//�������
    [SerializeField] private float giveDamageOf = 5f;
    [SerializeField] private float reloadTime = 3f;

    private float nextFireTime;
    private int currentMagzine;
    private int currentAmmo;

    [SerializeField] private bool isReloading;



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

                //�������������ʬ�壬����ʾ����
            }

            //NPC
            WalkCharacterAI characterNPC = hit.transform.GetComponent<WalkCharacterAI>();

            if (characterNPC != null)
            {
                characterNPC.CharacterHitDamage(giveDamageOf);

                //ѪҺЧ��

                //�������������ʬ�壬����ʾ����
            }
        }

        currentMagzine--;

        //�����������
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        //��������

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
