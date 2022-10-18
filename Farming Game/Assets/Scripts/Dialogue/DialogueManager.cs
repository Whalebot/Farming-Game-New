using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public static bool inDialogue;

    public bool dialogueActive;
    public GameObject dialogueWindow;
    bool sentenceFinished;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI dialogueText;
    public TextAnimatorPlayer dialoguePlayer;
    [SerializeField]
    public Queue<Dialogue> dialogues;
    public Image p1Image;
    public Image p2Image;
    public Color overlayColor;
    NPCSO p1SO, p2SO;
    public NPCSO[] npcs;

    Dialogue tempDialogue;
    public GameObject dialogueOptionsMenu;
    public GameObject dialogueOptionPrefab;
    public InventoryScript inventory;
    // public GameObject pressSFX;
    // public GameObject endSFX;

    public TextTags nameTags;
    public TextTags blueTag;


    public delegate void DialogueEvent();
    public DialogueEvent dialogueEnd;

    bool usedEvent;
    bool delay;
    float pressCounter;
    // Use this for initialization

    private void Awake()
    {
        Instance = this;
        dialogues = new Queue<Dialogue>();
    }

    void Start()
    {
        inventory = InventoryScript.Instance;
        InputManager.Instance.southInput += ButtonPress;
        InputManager.Instance.westInput += ButtonPress;


    }
    void Update()
    {

        if (dialogueActive)
        {
            dialogueWindow.SetActive(true);
            dialogueWindow.SetActive(!UIManager.prioritizeUI);
            if (!sentenceFinished && dialoguePlayer.textAnimator.allLettersShown) OnSentenceEnd();
        }
        else { dialogueWindow.SetActive(false); }

        if (pressCounter > 0)
            pressCounter -= Time.deltaTime;
    }

    void ButtonPress()
    {
        if (!dialogueActive) return;
        if (UIManager.prioritizeUI) return;


        //Instantiate(pressSFX, transform.position, Quaternion.identity);


        if (tempDialogue.eventScripts.Length > 0 && !usedEvent && sentenceFinished)
        {
            if (pressCounter > 0) return;
            usedEvent = true;
            foreach (EventScript tempEvent in tempDialogue.eventScripts)
            {

                switch (tempEvent.eventType)
                {
                    case EventScript.EventType.Items:
                        foreach (Item item in tempEvent.items)
                        {
                            inventory.PickupItem(item);
                        }

                        break;
                    case EventScript.EventType.Dialogue:
                        StartDialogue(tempEvent.eventDialogue.dialogues);

                        break;
                    case EventScript.EventType.Trigger:
                        DataManager.Instance.SetTrigger(tempEvent.triggerName);
                        break;
                    case EventScript.EventType.Tutorial:
                        DisplayNextSentence();
                        TutorialManager.Instance.StartTutorial(tempEvent.tutorial);
                        break;
                    case EventScript.EventType.Cost:

                        bool canAfford = inventory.CheckForItems(tempEvent.requiredItems) && GameManager.Instance.Money >= tempEvent.goldCost;
                        if (canAfford)
                        {
                            GameManager.Instance.Money -= tempEvent.goldCost;
                            inventory.RemoveItems(tempEvent.requiredItems);
                            StartDialogue(tempEvent.reward.dialogues);
                        }
                        else StartDialogue(tempEvent.fail.dialogues);
                        break;
                }
            }

        }
        else if (sentenceFinished)
        {
            if (pressCounter > 0) return;
            DisplayNextSentence();
        }
        else
        {
            FastForwardSentence();
        }

    }

    public void StartDialogue(Dialogue[] dialog)
    {

        if (delay) return;
        dialogueActive = true;
        inDialogue = true;

        dialogues.Clear();

        foreach (Dialogue d in dialog)
        {
            dialogues.Enqueue(d);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        pressCounter = 0.5F;
        if (dialogues.Count == 0)
        {
            if (tempDialogue.dialogueOptions.Length <= 0)
                EndDialogue();
            return;
        }

        usedEvent = false;

        tempDialogue = dialogues.Dequeue();

        p1Image.gameObject.SetActive(tempDialogue.character2 != Dialogue.Character.None);
        p2Image.gameObject.SetActive(tempDialogue.character != Dialogue.Character.None);
        if (tempDialogue.SFX != null) Instantiate(tempDialogue.SFX);




        if (tempDialogue.character != Dialogue.Character.None)
        {
            switch (tempDialogue.character)
            {
                case 0: p2Image.gameObject.SetActive(false); break;
                default: p2SO = npcs[(int)tempDialogue.character - 1]; break;
            }

            switch (tempDialogue.expression)
            {
                case Dialogue.Expression.Smiling: p2Image.sprite = p2SO.happySprite; break;
                case Dialogue.Expression.Blush: p2Image.sprite = p2SO.blushSprite; break;
                case Dialogue.Expression.Angry: p2Image.sprite = p2SO.angrySprite; break;
                case Dialogue.Expression.Smug: p2Image.sprite = p2SO.smugSprite; break;
                default: p2Image.sprite = p2SO.defaultSprite; break;

            }
        }
        if (tempDialogue.character2 != Dialogue.Character.None)
        {

            switch (tempDialogue.character2)
            {
                case 0: p1Image.gameObject.SetActive(false); break;
                default: p1SO = npcs[(int)tempDialogue.character2 - 1]; break;
            }
            switch (tempDialogue.expression2)
            {
                case Dialogue.Expression.Smiling: p1Image.sprite = p1SO.happySprite; break;
                case Dialogue.Expression.Blush: p1Image.sprite = p1SO.blushSprite; break;
                case Dialogue.Expression.Angry: p1Image.sprite = p1SO.angrySprite; break;
                case Dialogue.Expression.Smug: p1Image.sprite = p1SO.smugSprite; break;
                default: p1Image.sprite = p1SO.defaultSprite; break;
            }
        }

        switch (tempDialogue.highlight)
        {
            case Dialogue.Highlight.None:
                p1Image.color = overlayColor; p2Image.color = overlayColor;
                titleText.text = tempDialogue.character.ToString();
                break;
            case Dialogue.Highlight.P1:
                p1Image.color = Color.white; p2Image.color = overlayColor;
                titleText.text = tempDialogue.character2.ToString();
                break;
            case Dialogue.Highlight.P2:
                p1Image.color = overlayColor; p2Image.color = Color.white;
                titleText.text = tempDialogue.character.ToString();
                break;
            default:
                p1Image.color = Color.white; p2Image.color = Color.white;
                titleText.text = tempDialogue.character.ToString();
                break;
        }



        dialogueText.text = CheckStringTags(tempDialogue.sentence);
        dialoguePlayer.ShowText(CheckStringTags(tempDialogue.sentence));
        sentenceFinished = false;

        //StopAllCoroutines();
        //StartCoroutine(TypeSentence(tempDialogue.sentence));

    }

    void SetupDialogueOptions()
    {
        dialogueOptionsMenu.SetActive(true);
        GameObject GO = null;
        foreach (DialogueOption option in tempDialogue.dialogueOptions)
        {

            GameObject tempOption = Instantiate(dialogueOptionPrefab, dialogueOptionsMenu.transform);

            tempOption.GetComponent<ButtonOption>().eventScripts = option.eventScripts;
            tempOption.GetComponentInChildren<TextMeshProUGUI>().text = option.dialogueText;
            if (GO == null)
                GO = tempOption;
        }
        //eventSystem.firstSelectedGameObject = GO;
        UIManager.Instance.SetActive(GO);
    }

    public void ResetDialogueOptions()
    {
        dialogueOptionsMenu.SetActive(false);
        int children = dialogueOptionsMenu.transform.childCount;
        for (int i = 0; i < children; ++i)
            Destroy(dialogueOptionsMenu.transform.GetChild(i).gameObject);

    }

    IEnumerator TypeSentence(string sentence)
    {
        sentenceFinished = false;
        dialogueText.text = "";
        string converted = CheckStringTags(sentence);

        string temp = "";
        foreach (char letter in converted.ToCharArray())
        {
            temp += letter;
            dialogueText.text = (temp);
            yield return null;
        }
        dialogueText.text = CheckStringTags(tempDialogue.sentence);
        OnSentenceEnd();

    }

    void OnSentenceEnd()
    {

        if (tempDialogue.dialogueOptions.Length > 0)
        {
            SetupDialogueOptions();
            //StartDialogue(tempDialogue.dialogueOptions[0].eventScript.eventDialogue.dialogues);
        }
        sentenceFinished = true;
    }

    void FastForwardSentence()
    {

        StopAllCoroutines();

        dialoguePlayer.SkipTypewriter();
        OnSentenceEnd();
    }

    string CheckStringTags(string s)
    {
        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            foreach (string tag in blueTag.tags)
            {
                if (words[i].ToLower().Contains(tag))
                {
                    words[i] = "<color=#00ffffff>" + words[i] + "</color>";
                }
            }

            foreach (string tag in nameTags.tags)
            {
                if (words[i].ToLower().Contains(tag))
                {
                    words[i] = "<color=#2990ff>" + words[i] + "</color>";
                }
            }
        }
        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }

    IEnumerator DialogueDelay()
    {
        delay = true;
        yield return new WaitForSeconds(0.5F);
        delay = false;
    }

    public void EndDialogue()
    {
        dialogueEnd?.Invoke();
        inDialogue = false;
        dialogueActive = false;
        print("End Dialogue");
        StartCoroutine("DialogueDelay");
    }

}
