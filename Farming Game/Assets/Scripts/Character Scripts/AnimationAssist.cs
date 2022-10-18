using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAssist : MonoBehaviour
{

    CharacterSFX SFX;
    public float animationEventWeight = 0.5F;
    // Start is called before the first frame update
    void Start()
    {
        SFX = GetComponentInParent<CharacterSFX>();
    }

    void JumpFX() { }


    void Step(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            SFX.Step(evt);
        }
    }

}
