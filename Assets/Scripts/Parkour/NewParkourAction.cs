using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour Menu/Create New Parkour Action")]
public class NewParkourAction : ScriptableObject
{

    [SerializeField] private string animationName;
    [SerializeField] private float minimumHeight;
    [SerializeField] private float maximumHeight;

}
