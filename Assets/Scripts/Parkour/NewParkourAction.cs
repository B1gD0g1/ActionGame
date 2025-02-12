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
    [SerializeField] private float postActionDelay;

    [Header("Ä¿±êÆ¥Åä TargetMatching")]
    [SerializeField] private bool enableTargetMatching = true;
    [SerializeField] private AvatarTarget matchBodyPart;
    [SerializeField] private float matchStartTime;
    [SerializeField] private float matchTargetTime;
    [SerializeField] private Vector3 matchPositionWeight = new Vector3(0, 1 ,0);


    public Quaternion TargetRotation { get; set; }

    public Vector3 MatchPosition { get; set; }



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

            if (enableTargetMatching)
            {
                MatchPosition = hitData.heightHitInfo.point;
            }

            return true;
        }
    }

    public string AnimationName => animationName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;

    public bool EnableTargetMatching  => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
}
