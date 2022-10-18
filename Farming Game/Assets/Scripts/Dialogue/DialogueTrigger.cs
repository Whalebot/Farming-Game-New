using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueTrigger : MonoBehaviour {
    public Dialogue[] dialogues;





   public bool trigger;
    public bool started;

	public void Start(){
	}

    private void Update()
    {
        if (trigger && ! started) TriggerDialogue();
    }

    public void TriggerDialogue() {
        started = true;
        GameObject.FindGameObjectWithTag("Manager").GetComponent<DialogueManager>().StartDialogue(dialogues);
    }
    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) { TriggerDialogue(); Destroy(gameObject); }
    }

}
