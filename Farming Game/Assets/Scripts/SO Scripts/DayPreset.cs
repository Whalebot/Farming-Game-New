using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "Day Preset", menuName = "ScriptableObjects/DayPreset")]
public class DayPreset : ScriptableObject
{
    public float sunIntensity;
    public float moonIntensity;

    [FoldoutGroup("Settings")] public AnimationCurve timeCurve;
    [FoldoutGroup("Settings")]
    public Gradient sunGradient;
    [FoldoutGroup("Settings")]
    [GradientUsage(true)]
    public Gradient skyGradient;
    [FoldoutGroup("Settings")]
    [GradientUsage(true)]
    public Gradient horizonGradient;
    [FoldoutGroup("Settings")]
    [GradientUsage(true)]
    public Gradient groundGradient;

    [FoldoutGroup("Fog")] public Gradient fogGradient;
    [FoldoutGroup("Fog")] public AnimationCurve fogDensity;

    public AnimationCurve skyExponentCurve;
    public AnimationCurve cloudScaleCurve;
    public AnimationCurve temperatureCurve;
}

