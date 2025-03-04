using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private new Camera camera;
    [SerializeField] private Transform playerTransform;
    private PlayerMovement playerMovement;


    //��������ᣬ���ڿ��ƴ�ֱ��ת���ӽ����¿���
    [SerializeField] Transform cameraPivot;
    //���ڴ洢�������ǰλ����Ŀ��λ��֮����ٶȲ�
    [SerializeField] private Vector3 camFollowVelocity = Vector3.zero;

    [Header("������ƶ�����ת")]
    //�����ƽ��������ٶ�
    [SerializeField] private float camFollowSpeed = 0f;
    //�����ˮƽ��ת���ٶ�
    [SerializeField] private float camLookSpeed = 0.1f;
    //�������ֱ��ת���ٶ�
    [SerializeField] private float camPivotSpeed = 0.1f;

    [SerializeField] private float lookAngle;
    [SerializeField] private float pivotAngle;
    //���ƴ�ֱ��ת�ķ�Χ������������������ϻ����¿�
    [SerializeField] private float minimumPivotAngle = -30f;
    [SerializeField] private float maximumPivotAngle = 30f;

    [Header("��׼����")]
    [SerializeField] private float scopedFOV;
    [SerializeField] private float defaultFOV;


    private void Awake()
    {
        //��������꣬ʹ�䲻���Ƴ���Ϸ���ڣ������ع��
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

        //����ˮƽ��ת�Ƕ�
        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        //���´�ֱ��ת�Ƕ�
        pivotAngle -= inputManager.cameraInputY * camPivotSpeed;

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        //����ˮƽ��ת
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //���ô�ֱ��ת
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
