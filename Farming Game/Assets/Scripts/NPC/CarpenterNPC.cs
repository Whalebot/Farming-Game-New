using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpenterNPC : NPCInteract
{


    public override void LoadNPC()
    {
        //triggeredDialogue = DataManager.Instance.currentSaveData.triggers.vaillantTriggerStep;
    }

    public override void SaveNPC()
    {
       // DataManager.Instance.currentSaveData.triggers.vaillantTriggerStep = triggeredDialogue;
    }
}
