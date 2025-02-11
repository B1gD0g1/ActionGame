using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour Menu/Create New Parkour Action")]
public class NewParkourAction : ScriptableObject
{

    [SerializeField] private string animationName;
    [SerializeField] private float minimumHeight;
    [SerializeField] private float maximumHeight;

    [SerializeField] private bool rotateToObstacle;

    public Quaternion TargetRotation { get; set; }

    public bool CheckIfAvailable(ObstacleInfo hitData, Transform player)
    {
        float checkHeight = hitData.heightHitInfo.point.y - player.position.y;

        if (checkHeight < minimumHeight || checkHeight > maximumHeight)
        {
            return false;
        }
        else
        {
            if (rotateToObstacle)
            {
                TargetRotation = Quaternion.LookRotation(-hitData.hitInfo.normal);
            }

            return true;
        }
    }

    public string AnimationName => animationName;
    public bool RotateToObstacle => rotateToObstacle;
}
