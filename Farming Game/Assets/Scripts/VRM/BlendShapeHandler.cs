using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using VRM;
public class BlendShapeHandler : MonoBehaviour
{
    public BlendShapeAvatar blendShapeAvatar;
    [FoldoutGroup("Clips")]
    [InlineEditor] public BlendShapeClip blendShapeClip;
    //[Space(100)]
    public SkinnedMeshRenderer mr;
    [Range(0, 1)] public float blinkValue = 0;
    public float smoothSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        SetBlendShape(blinkValue);
    }
    void OnValidate()
    {
        SetBlendShape(blinkValue);
    }
    void SetBlendShape(float val)
    {
        foreach (var item in blendShapeClip.Values)
        {
            //    Debug.Log(item.ToString());
            mr.SetBlendShapeWeight(item.Index, Mathf.Clamp01(item.Weight * val));
        }
    }
    void SetBlendShape(BlendShapeClip clip, float val)
    {
        foreach (var item in clip.Values)
        {
            //    Debug.Log(item.ToString());
            mr.SetBlendShapeWeight(item.Index, Mathf.Clamp01(item.Weight * val));
        }
    }

    [Button]
    void SetSmoothBlendShape(BlendShapeClip clip) {
        StartCoroutine(SmoothBlendShape(clip));
    }

    IEnumerator SmoothBlendShape(BlendShapeClip clip)
    {
        float val = 0;
        while (val < 1)
        {
            val += smoothSpeed;
            SetBlendShape(clip, val);
            yield return new WaitForFixedUpdate();
        }
        SetBlendShape(clip, 1);
    }

    [Button]
    void SetBlendShape()
    {
        foreach (var item in blendShapeClip.Values)
        {
            Debug.Log(item.ToString());
            mr.SetBlendShapeWeight(item.Index, item.Weight);
        }

    }

    [Button]
    void ResetBlendShapes()
    {
        for (int i = 0; i < mr.sharedMesh.blendShapeCount; i++)
        {
            mr.SetBlendShapeWeight(i, 0);
        }
    }
}
