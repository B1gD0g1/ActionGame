using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator animator;
    private PlayerMovement playerMovement;


    public GameObject pistol;
    public GameObject rifle;

    private int currentWeaponIndex = 0;
    private string pistolAnimBool = "pistolActive";
    private string rifleAnimBool = "rifleActive";

    private GameObject[] weapons;
    private string[] weaponAnimBools;


    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        weapons = new GameObject[] { pistol, rifle };
        weaponAnimBools = new string[] { pistolAnimBool, rifleAnimBool };
    }

    private void Update()
    {
        if (inputManager.GetChangeRifleInput())
        {
            CycleWeapons();
        }
    }

    private void CycleWeapons()
    {
        DeactivateAllWeapons();

        currentWeaponIndex = (currentWeaponIndex + 1) % (weapons.Length + 1);

        if (currentWeaponIndex < weapons.Length)
        {
            GameObject currentWeapon = weapons[currentWeaponIndex];
            string currentAnimBool = weaponAnimBools[currentWeaponIndex];

            currentWeapon.SetActive(true);
            animator.SetBool(currentAnimBool, true);
        }
    }

    private void DeactivateAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            weapon.SetActive(false);
        }

        foreach (var animBool in weaponAnimBools)
        {
            animator.SetBool(animBool, false);
        }
    }
}
