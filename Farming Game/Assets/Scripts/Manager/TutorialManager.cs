using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public bool inTutorial;
    public GameObject startFX;
    public GameObject endFX;
    public List<TutorialSO> tutorialQueue;
    public float delay;
    float counter;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.southInput += EndTutorial;
        InputManager.Instance.westInput += EndTutorial;
    }

    private void FixedUpdate()
    {
        if (counter > 0)
        {
            counter -= Time.fixedDeltaTime;
        }

        if (GameManager.isPaused || GameManager.inventoryMenuOpen) return;
        if (tutorialQueue.Count > 0)
        {
            ActivateTutorial();
        }
    }

    void ActivateTutorial()
    {
        counter = delay;
        inTutorial = true;

        Instantiate(tutorialQueue[0].tutorialPanel, transform);
        Instantiate(startFX);
        tutorialQueue.RemoveAt(0);
    }

    public void StartTutorial(TutorialSO tutorial)
    {

        tutorialQueue.Add(tutorial);

    }

    public void EndTutorial()
    {
        if (!inTutorial || counter > 0) return;
        inTutorial = false;

        Instantiate(endFX);
        int children = transform.childCount;

        for (int i = 0; i < children; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
