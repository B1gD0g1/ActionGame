using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private Transform playerTransform;

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


    private void Awake()
    {
        //锁定鼠标光标，使其不能移出游戏窗口，并隐藏光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerTransform = FindObjectOfType<PlayerManager>().transform;
    }

    private void Update()
    {
        inputManager = FindObjectOfType<InputManager>();

    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, ref camFollowVelocity, camFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        //更新水平旋转角度
        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        //更新垂直旋转角度
        pivotAngle = pivotAngle - (inputManager.cameraInputY * camPivotSpeed);
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
    }
}
