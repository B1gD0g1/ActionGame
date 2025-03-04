using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private new Camera camera;
    [SerializeField] private Transform playerTransform;
    private PlayerMovement playerMovement;


    //摄像机枢轴，用于控制垂直旋转（视角上下看）
    [SerializeField] Transform cameraPivot;
    //用于存储摄像机当前位置与目标位置之间的速度差
    [SerializeField] private Vector3 camFollowVelocity = Vector3.zero;

    [Header("摄像机移动和旋转")]
    //摄像机平滑跟随的速度
    [SerializeField] private float camFollowSpeed = 0f;
    //摄像机水平旋转的速度
    [SerializeField] private float camLookSpeed = 0.1f;
    //摄像机垂直旋转的速度
    [SerializeField] private float camPivotSpeed = 0.1f;

    [SerializeField] private float lookAngle;
    [SerializeField] private float pivotAngle;
    //限制垂直旋转的范围，避免摄像机过度向上或向下看
    [SerializeField] private float minimumPivotAngle = -30f;
    [SerializeField] private float maximumPivotAngle = 30f;

    [Header("瞄准设置")]
    [SerializeField] private float scopedFOV;
    [SerializeField] private float defaultFOV;


    private void Awake()
    {
        //锁定鼠标光标，使其不能移出游戏窗口，并隐藏光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputManager = FindObjectOfType<InputManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerTransform = FindObjectOfType<InputManager>().transform;
    }

    private void Update()
    {

    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleScopedFOV();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, 
            ref camFollowVelocity, camFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        //更新水平旋转角度
        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        //更新垂直旋转角度
        pivotAngle -= inputManager.cameraInputY * camPivotSpeed;

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        //设置水平旋转
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //设置垂直旋转
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

        if (playerMovement.isScoped == true)
        {
            camLookSpeed = 0.2f;
            camPivotSpeed = 0.2f;
            minimumPivotAngle = -10f;
            maximumPivotAngle = 10f;

            //playerTransform.rotation = Quaternion.Euler(pivotAngle, lookAngle, 0);
        }
        else
        {
            camLookSpeed = 0.1f;
            camPivotSpeed = 0.1f;
            minimumPivotAngle = -30f;
            maximumPivotAngle = 30f;
        }
    }

    private void HandleScopedFOV()
    {
        if (inputManager.GetScopeInput())
        {
            camera.fieldOfView = scopedFOV;
            playerMovement.isScoped = true;
        }
        else
        {
            camera.fieldOfView = defaultFOV;
            playerMovement.isScoped = false;
        }
    }

    public float GetLookAngle()
    {
        return lookAngle;
    }

    public float GetPivotAngle()
    {
        return pivotAngle;
    }
}
