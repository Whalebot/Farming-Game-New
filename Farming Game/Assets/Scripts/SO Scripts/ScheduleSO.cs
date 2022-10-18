using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ScheduleSO", menuName = "ScriptableObjects/Schedule", order = 1)]

public class ScheduleSO : ScriptableObject
{
    public Transform defaultTransform;
    public NPCPose defaultPose;
    public NPCPose[] dailyPoses;
}
