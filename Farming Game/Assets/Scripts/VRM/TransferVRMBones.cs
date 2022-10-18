using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using Sirenix.OdinInspector;

public class TransferVRMBones : MonoBehaviour
{
    public VRMSpringBone[] springBones;

    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    void GetAllSpringBones()
    {
        springBones = GetComponentsInChildren<VRMSpringBone>();
    }

    [Button]
    void TryToChangeRootBones()
    {

        foreach (VRMSpringBone item in springBones)
        {
            //List<Transform> bonesToReplace = new List<Transform>();
            //foreach (Transform bone in item.RootBones)
            //{
            //    if (!bone.IsChildOf(transform))
            //    {
            //        bonesToReplace.Add(bone);
            //    }
            //}
            for (int i = 0; i < item.RootBones.Count; i++)
            {
                string boneName = item.RootBones[i].name;
                if (!item.RootBones[i].IsChildOf(transform))
                {


                    if (ReturnChildOfName(boneName) != null)
                        item.RootBones[i] = ReturnChildOfName(boneName);
                }
            }

        }
    }

    Transform ReturnChildOfName(string s)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in allChildren)
        {
            if (item.name == s)
                return item;
          
        }
        if (ReturnStringName(s) != "")
        {
            return ReturnChildOfNameNoLoop(ReturnStringName(s));
        }
        return null;
    }

    Transform ReturnChildOfNameNoLoop(string s)
    {
        Debug.Log(s);
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in allChildren)
        {
            if (item.name == s)
                return item;
        }
        return null;
    }

    string ReturnStringName(string s)
    {
        if (s.Contains("L_Bust1")) return "Breast_L";
        if (s.Contains("R_Bust1")) return "Breast_R";

        return "";
    }
}
