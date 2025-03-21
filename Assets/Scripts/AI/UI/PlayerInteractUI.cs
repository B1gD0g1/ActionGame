using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;

    [SerializeField] private new Camera camera;

    private void Update()
    {
        transform.forward = camera.transform.forward;

        if (playerInteract.GetInteractableObject() != null)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        containerGameObject.SetActive(true);
    } 

    private void Hide()
    {
        containerGameObject.SetActive(false);
    }
}
