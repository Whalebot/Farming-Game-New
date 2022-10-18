using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : Interactable
{
    //public NPCPose defaultPose;
    public NPCPose[] dailyPoses;
    Animator anim;
    public int dialogueStep;
    public int triggeredDialogue;
    public enum NPCState
    {

    }

    public DialogueSO[] scriptedDialogue;
    public DialogueSO[] defaultDialogues;

    public EventDialogue[] eventDialogues;

    public bool trigger;
    public bool started;

    // Start is called before the first frame update
    void Awake()
    {
        DataManager.Instance.loadDataEvent += LoadNPC;
        DataManager.Instance.saveDataEvent += SaveNPC;
        anim = GetComponentInChildren<Animator>();
    }

    private void OnDisable()
    {
        DataManager.Instance.loadDataEvent -= LoadNPC;
        DataManager.Instance.saveDataEvent -= SaveNPC;
    }

    private void Start()
    {
        //if (defaultPose.t != null)
        //{
        //    ApplyPose(defaultPose);
        //}
        if (dailyPoses.Length >= TimeManager.Instance.date)
            if (dailyPoses[TimeManager.Instance.date - 1].t != null)
            {
                ApplyPose(dailyPoses[TimeManager.Instance.date - 1]);
            }
    }

    public void ApplyPose(NPCPose pose)
    {

        transform.position = pose.t.position;
        transform.rotation = pose.t.rotation;
        anim.SetInteger("IdleID", pose.animationID);
        anim.SetTrigger("Trigger");

        foreach (var item in pose.enableObjects)
        {
            item.SetActive(true);
        }

    }

    public virtual void LoadNPC()
    {
        //  print("load lecia");
       // triggeredDialogue = DataManager.Instance.currentSaveData.triggers.leciaTriggerStep;
    }

    public virtual void SaveNPC()
    {
        //DataManager.Instance.currentSaveData.triggers.leciaTriggerStep = triggeredDialogue;
    }

    public override void Interact()
    {
        print("DialogueStart");
        trigger = false;
        //started = true;

        if (CheckForTimeEvents() != null)
        {
            DialogueManager.Instance.StartDialogue(CheckForTimeEvents().dialogues);
            return;
        }

        if (triggeredDialogue < dialogueStep)
        {
            if (scriptedDialogue.Length > dialogueStep)
            {
                DialogueManager.Instance.StartDialogue(scriptedDialogue[dialogueStep].dialogues);
                triggeredDialogue++;
            }
        }
        else
        {
            if (dailyPoses.Length >= TimeManager.Instance.date)
                if (dailyPoses[TimeManager.Instance.date - 1].dialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(dailyPoses[TimeManager.Instance.date - 1].dialogue.dialogues);
                    return;
                }
            DialogueManager.Instance.StartDialogue(defaultDialogues[Random.Range(0, defaultDialogues.Length)].dialogues);
        }
        SaveNPC();
    }

    public DialogueSO CheckForTimeEvents()
    {
        foreach (EventDialogue d in eventDialogues)
        {
            if (TimeManager.Instance.season == d.timeEvent.season && TimeManager.Instance.date == d.timeEvent.date)
            {

                return d.dialogue;
            }
        }

        return null;
    }
}

[System.Serializable]
public class EventDialogue
{
    public DialogueSO dialogue;
    public TimeEvent timeEvent;
}

[System.Serializable]
public class NPCPose
{
    public Transform t;
    public int animationID;
    public DialogueSO dialogue;
    public GameObject[] enableObjects;
}