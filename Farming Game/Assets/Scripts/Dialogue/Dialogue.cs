using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialogue {
    public enum Character { None, Traveller, Lecia, Vaillant, Carpenter};
    public Character character = new Character();

    public enum Expression { Default, Smiling, Angry, Smug, Blush };
    public Expression expression = new Expression();

    public enum Highlight { Both, P1, P2 , None};
    public Highlight highlight = new Highlight();

    public Character character2 = new Character();
    public Expression expression2 = new Expression();

    public GameObject SFX;

    [TextArea(3,10)]
    public string sentence;
    public EventScript[] eventScripts;
    public DialogueOption[] dialogueOptions;
}

[System.Serializable]
public class DialogueOption {
    [TextArea(3, 10)]
    public string dialogueText;
    public EventScript[] eventScripts;
}
