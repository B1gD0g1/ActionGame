using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private Animator animator;


    [SerializeField] private Transform firePoint;

    [Header("武器数值")]
    [SerializeField] private float fireRate = 1f;//射速
    [SerializeField] private int magzineCapacity = 30;//弹匣容量
    [SerializeField] private int maxAmmo = 300;
    [SerializeField] private float fireRange = 100f;//射击长度
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
            Debug.Log("击中" + hit.transform.name);
        }

        currentMagzine--;
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        //换弹声音

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
