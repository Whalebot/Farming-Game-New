using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool isPaused;
    public static bool cutscene;
    public static bool inventoryMenuOpen;
    public static bool shopOpen;
    public static bool menuOpen;
    public static bool gameOver;

    public bool runNormally;
    public GameObject playerUI;

    public static float inGameTime;
    public float timeDisplay;

    public bool showNames = true;
    public bool showHPBar = true;

    static bool saveOnce = false;

    public float restartDelay = 5;

    public int gameFrameCount;
    public event Action advanceGameState;
    public event Action resetEvent;
    public delegate void GameEvent();
    public GameEvent playerDeath;

    public Transform player;

    public Character playerCharacter;
    public Character startCharacter;
    public bool hideMouse;

    [FoldoutGroup("Feedback")] public bool showDamageText;
    [FoldoutGroup("Feedback")] public float numberOffset;
    [FoldoutGroup("Feedback")] public float offsetResetSpeed;
    [FoldoutGroup("Feedback")] float offsetCounter;
    [FoldoutGroup("Feedback")] public GameObject damageText;
    [FoldoutGroup("Feedback")] public GameObject healingText;

    public bool showHitboxes;
    bool reloading;

    [FoldoutGroup("Feedback")] public float slowMotionValue;
    [FoldoutGroup("Feedback")] public float slowMotionDuration;
    [FoldoutGroup("Feedback")] public float slowMotionSmoothing;
    float startTimeStep;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        if (!saveOnce)
        {
            saveOnce = true;

        }

        //AIManager.Instance.allEnemiesKilled += FinalHit;
        startTimeStep = Time.fixedDeltaTime;
    }

    public int Money
    {
        get
        {
            return DataManager.Instance.currentSaveData.profile.gold;

        }
        set
        {
            DataManager.Instance.currentSaveData.profile.gold = value;
            // UIManager.Instance.UpdateMoney();

        }
    }

    public void AdvanceGameState()
    {

        Physics.autoSimulation = runNormally;
        advanceGameState?.Invoke();

        gameFrameCount++;
        //frameCounterEvent?.Invoke(gameFrameCount);
    }

    [Button]
    public void AdvanceGameStateButton()
    {
        runNormally = false;
        Physics.autoSimulation = false;
        AdvanceGameState();
        Physics.Simulate(Time.fixedDeltaTime);

    }

    public void SaveData()
    {
        //  SaveManager.Instance.SaveData();
    }



    public void KillPlayer()
    {
        playerDeath?.Invoke();
    }

    public void SetTrigger(string n)
    {
        //  GameTriggers triggers = DataManager.Instance.currentSaveData.triggers;

        //FieldInfo[] defInfo = triggers.GetType().GetFields();

        //for (int i = 0; i < defInfo.Length; i++)
        //{

        //    object obj = triggers;
        //    if (defInfo[i].GetValue(obj) is bool)
        //    {

        //        if (defInfo[i].Name.Contains(n))
        //        {
        //            defInfo[i].SetValue(obj, true);
        //        }
        //    }
        //}
    }


    public void DamageNumbers(Transform other, int damageValue)
    {
        if (showDamageText)
        {
            GameObject text = Instantiate(damageText, other.position + Vector3.up * offsetCounter, Quaternion.identity);
            text.GetComponentInChildren<TextMeshProUGUI>().text = "" + damageValue;
            offsetCounter += numberOffset;
        }
    }


    public void Slowmotion(float dur)
    {
        StartCoroutine(SetSlowmotion( dur));
    }

    public void FinalHit()
    {

        CameraManager.Instance.SetGroupTarget();
    }

    IEnumerator DelayResume()
    {
        yield return new WaitForFixedUpdate();
        isPaused = false;
    }

    public void PauseGame()
    {
        isPaused = true;

        //Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        // print("Resume");

        //  Time.timeScale = 1;
        StartCoroutine("DelayResume");
    }

    IEnumerator SetSlowmotion(float dur)
    {
        Time.timeScale = slowMotionValue;
        Time.fixedDeltaTime = startTimeStep * Time.timeScale;
        yield return new WaitForSecondsRealtime(dur);
        StartCoroutine("RevertSlowmotion");
    }
    IEnumerator RevertSlowmotion()
    {
        //gameOver = true;
        //CameraManager.Instance.RevertCamera();
        while (Time.timeScale < 1 && !isPaused)
        {
            Time.timeScale = Time.timeScale + slowMotionSmoothing;

            Time.fixedDeltaTime = startTimeStep * Time.timeScale;
            yield return new WaitForEndOfFrame();
        }


        Time.timeScale = 1;
        Time.fixedDeltaTime = startTimeStep;

    }

    private void Update()
    {
        //if (isPaused) { playerUI.SetActive(false); return; }
        //else { playerUI.SetActive(true); }
        isPaused = (UIManager.prioritizeUI || DialogueManager.inDialogue || TutorialManager.Instance.inTutorial || cutscene);

    }
    void FixedUpdate()
    {
        if (isPaused)
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            return;
        }
        if (hideMouse)
        {
            //Cursor.visible = menuOpen;
            //Cursor.lockState = CursorLockMode.Confined;
        }

        if (runNormally)
        {
            AdvanceGameState();
        }

        menuOpen = inventoryMenuOpen || shopOpen || StorageScript.inStorage;
        inGameTime += Time.fixedDeltaTime;
        timeDisplay = inGameTime;


        if (offsetCounter > 0)
        {
            offsetCounter -= Time.fixedDeltaTime * offsetResetSpeed;
            if (offsetCounter <= 0) offsetCounter = 0;
        }

        if (gameOver && !reloading)
        {
            LoseGame();
        }
    }


    public void LoseGame()
    {
        TimeManager.Instance.Sleep();
        playerDeath?.Invoke();
        reloading = true;
        StartCoroutine("DeathScreenDelay");
        StartCoroutine("RestartDelay");
    }

    public void Sleep()
    {
        TimeManager.Instance.Sleep();
        reloading = true;
        StartCoroutine("DeathScreenDelay");
        //  TransitionManager.Instance.LoadHouse();
    }

    void RestartGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        inGameTime = 0;
    }

    IEnumerator DeathScreenDelay()
    {
        yield return new WaitForSeconds(0.5F);
        //TransitionManager.Instance.DeathScreen();
    }

    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        TimeManager.Instance.DayPass();
        DataManager.Instance.SaveData();
        // TransitionManager.Instance.LoadHouse();
    }

    public void ReturnToHub()
    {
        DataManager.Instance.SaveData();
        //TransitionManager.Instance.LoadHub();
    }

}
