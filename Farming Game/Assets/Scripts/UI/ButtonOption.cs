using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOption : MonoBehaviour
{
    Button thisButton;
    public EventScript[] eventScripts;
    // Start is called before the first frame update
   // public DialogueManager dialogueManager;
    public GameObject hoverSFX;
    public GameObject clickSFX;

    bool pressOnce;
    void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(ButtonPress);

      //  dialogueManager = DialogueManager.Instance;
    }


    public void TriggerEvent()
    {
        //foreach (EventScript events in eventScripts)
        //{
        //    switch (events.eventType)
        //    {
        //        case EventScript.EventType.Dialogue:
        //            dialogueManager.StartDialogue(events.eventDialogue.dialogues);
        //            break;
        //        case EventScript.EventType.Items:

        //            break;
        //        default:
        //            dialogueManager.EndDialogue();
        //            break;
        //    }
        //}
        //if (eventScripts.Length <= 0) dialogueManager.EndDialogue();
        //dialogueManager.ResetDialogueOptions();
    }

    // Update is called once per frame
    public void OnHover()
    {
        Instantiate(hoverSFX);
    }

    public void ButtonPress()
    {
        if (!pressOnce)
        {
            Instantiate(clickSFX);
            pressOnce = true;
            TriggerEvent();
        }

    }
}
