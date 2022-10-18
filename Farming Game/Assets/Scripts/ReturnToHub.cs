using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReturnToHub : MonoBehaviour
{
    public GameObject window;
    public GameObject yesButton;
    public GameObject noButton;


    private void Start()
    {
        InputManager.Instance.selectInput += OpenTab;
    }

    void OpenTab()
    {
        UIManager.prioritizeUI = true;
        window.SetActive(true);
        UIManager.Instance.SetActive(yesButton);

    }

    private void OnEnable()
    {
       
    }

    public void LoadHub() {
        StartPosition.spawnPointID = 2;
        UIManager.prioritizeUI = false;
        TransitionManager.Instance.LoadScene(0);

    }
    public void ClosePrompt() {
        window.SetActive(false);
        UIManager.prioritizeUI = false;
    }
}
