using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    [Header("…„œÒª˙")]
    [SerializeField] private GameObject camera1;
    [SerializeField] private GameObject camera2;
    [SerializeField] private GameObject camera3;
    [SerializeField] private GameObject camera4;
    [SerializeField] private GameObject playerCamera;


    [Header("Anim")]
    private Animator animCamera1;
    private Animator animCamera2;
    private Animator animCamera3;
    private Animator animCamera4;


    private bool camera1Finished = false;
    private bool camera2Finished = false;
    private bool camera3Finished = false;
    private bool camera4Finished = false;

    //“Ù∆µ


    private void Start()
    {
        animCamera1 = camera1.GetComponent<Animator>();
        animCamera2 = camera2.GetComponent<Animator>();
        animCamera3 = camera3.GetComponent<Animator>();
        animCamera4 = camera4.GetComponent<Animator>();

        ActiveCamera(camera1);
        playerCamera.SetActive(false);
    }

    private void ActiveCamera(GameObject cameraToActivate)
    {
        camera1.SetActive(cameraToActivate == camera1);
        camera2.SetActive(cameraToActivate == camera2);
        camera3.SetActive(cameraToActivate == camera3);
        camera4.SetActive(cameraToActivate == camera4);

        if (cameraToActivate == camera1) camera1Finished = false;
        if (cameraToActivate == camera2) camera2Finished = false;
        if (cameraToActivate == camera3) camera3Finished = false;
        if (cameraToActivate == camera4) camera4Finished = false;
    }

    private void Update()
    {
        if (camera1.activeSelf && !camera1Finished 
            && animCamera1.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1
            && !animCamera1.IsInTransition(0))
        {
            camera1Finished = true;
            ActiveCamera(camera2);
        }

        if (camera2.activeSelf && !camera2Finished
            && animCamera2.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1
            && !animCamera2.IsInTransition(0))
        {
            camera2Finished = true;
            ActiveCamera(camera3);
        }

        if (camera3.activeSelf && !camera3Finished
            && animCamera3.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1
            && !animCamera3.IsInTransition(0))
        {
            camera3Finished = true;
            ActiveCamera(camera4);
        }

        if (camera4.activeSelf && !camera4Finished
            && animCamera4.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1
            && !animCamera4.IsInTransition(0))
        {
            camera4Finished = true;
            camera4.SetActive(false);

            playerCamera.SetActive(true);
        }
    }
}
