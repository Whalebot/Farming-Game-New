using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public Animator fadeAnimator;
    public Animator deathAnimator;
    public Animator winAnimator;


    public static bool isLoading;

    public float sceneTransitionDelay;
    //  public GameObject resultScreen;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isLoading = false;
    }

    public void LoadHub()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator TransitionDelay(int sceneIndex)
    {
        AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        DataManager.Instance.SaveData();
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadHouse()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine("SleepDelay", 1);
    }

    IEnumerator SleepDelay(int sceneIndex)
    {
        AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        TimeManager.Instance.DayPass();
        DataManager.Instance.SaveData();
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(int sceneIndex)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(TransitionDelay(sceneIndex));
    }


    public void ResultScreen()
    {
        //  resultScreen.SetActive(true);
    }

    public void DeathScreen()
    {
        deathAnimator.SetTrigger("Trigger");
    }


    public void WinScreen()
    {
        winAnimator.SetTrigger("Trigger");
    }

    public void Fade()
    {
        fadeAnimator.SetTrigger("Trigger");
    }
}
