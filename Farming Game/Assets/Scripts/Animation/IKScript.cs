using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FIMSpace.FLook;
using VRM;

public class IKScript : MonoBehaviour
{
    //public FLookAnimator fLook;
    public Transform defaultTarget;
    public bool disableWhenInAnimation;
    public float smooth;
    Movement mov;
    float zeroFloat;

    public bool isHumanoid = true;
    public bool IKActive;
    public Transform lookTarget;
    public float lookWeight;
    float currentLookWeight;

    VRMLookAtHead vrm;


    // Start is called before the first frame update
    void Start()
    {
        mov = GetComponentInParent<Movement>();
        vrm = GetComponentInChildren<VRMLookAtHead>();
        mov.strafeSet += StrafeStart;
        mov.strafeBreak += StrafeBreak;
    }

    private void Update()
    {
        //if (fLook.ObjectToFollow == null) SetLookTarget(defaultTarget);

        //if (disableWhenInAnimation)
        //{
        //    if (mov.status.currentState == Status.State.InAnimation) fLook.LookAnimatorAmount = Mathf.SmoothDamp(fLook.LookAnimatorAmount, 0, ref zeroFloat, smooth);
        //    else
        //        fLook.LookAnimatorAmount = Mathf.SmoothDamp(fLook.LookAnimatorAmount, 1, ref zeroFloat, smooth);
        //}


    }

    void StrafeStart()
    {
        SetLookTarget(mov.strafeTarget);
    }

    void StrafeBreak()
    {
        SetLookTarget(defaultTarget);
    }

    public void SetLookTarget(Transform t)
    {

       // fLook.ObjectToFollow = t;
        if (!isHumanoid) return;
      //  vrm.Target = t;
        lookTarget = t;
    }

}