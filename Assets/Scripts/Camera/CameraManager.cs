using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private Transform playerTransform;

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


    private void Awake()
    {
        //��������꣬ʹ�䲻���Ƴ���Ϸ���ڣ������ع��
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

        //����ˮƽ��ת�Ƕ�
        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        //���´�ֱ��ת�Ƕ�
        pivotAngle = pivotAngle - (inputManager.cameraInputY * camPivotSpeed);
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
    }
}
