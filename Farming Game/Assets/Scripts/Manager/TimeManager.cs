using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public Vector2 clockTime;
    public float minuteMultiplier;
    public float damping;
    DataManager dataManager;



    public Season season = new Season();

    public WeekDay weekDay = new WeekDay();

    public int date;

    private float time;
    private float nextActionTime = 0.0f;

    public delegate void TimeEvent();
    public TimeEvent dayPassEvent;
    public TimeEvent hubDayEvent;
    public TimeEvent resetEvent;
    public TimeEvent sleepStartEvent;
    public TimeEvent sleepEvent;

    private void Awake()
    {
        Instance = this;
        dataManager = DataManager.Instance;
        dataManager.saveDataEvent += SaveTime;
        dataManager.loadDataEvent += LoadTime;
    }

    void Start()
    {
        // UpdateUI();

        if (SceneManager.GetActiveScene().name == "Hub")
        {
            while (DataManager.Instance.currentSaveData.savedDays[dataManager.sceneIndex] > 0)
            {
                hubDayEvent?.Invoke();
                resetEvent?.Invoke();
                DataManager.Instance.currentSaveData.savedDays[dataManager.sceneIndex]--;
            }
        }
    }

    void SaveTime()
    {
        dataManager.currentSaveData.profile.date = date;
        dataManager.currentSaveData.profile.time = clockTime;
        dataManager.currentSaveData.profile.season = season;

    }

    void LoadTime()
    {
        date = dataManager.currentSaveData.profile.date;
        clockTime = dataManager.currentSaveData.profile.time;
        season = dataManager.currentSaveData.profile.season;
    }


    private void FixedUpdate()
    {
        if (Keyboard.current.numpadPlusKey.wasPressedThisFrame) Sleep();
        if (!GameManager.isPaused)
        {
            time += Time.fixedDeltaTime;

            if (time > nextActionTime)
            {
                nextActionTime += 1;
                AddSecond();
            }
        }
    }

    public void Sleep()
    {
        sleepStartEvent?.Invoke();
    }

    [Button]
    public void DayPass()
    {
        clockTime.x = 8;
        clockTime.y = 0;

        date++;




        if (date > 7)
        {
            date = 1;
            SeasonChange();
        }
        switch (date)
        {
            case 1:
                weekDay = WeekDay.Monday;
                break;
            case 2:
                weekDay = WeekDay.Tuesday;
                break;
            case 3:
                weekDay = WeekDay.Wednesday;
                break;
            case 4:
                weekDay = WeekDay.Thursday;
                break;
            case 5:
                weekDay = WeekDay.Friday;
                break;
            case 6:
                weekDay = WeekDay.Saturday;
                break;
            case 7:
                weekDay = WeekDay.Sunday;
                break;

        }


        //if (SceneManager.GetActiveScene().name != "Hub")
        //{
        //    DataManager.Instance.currentSaveData.savedDays++;
        //}
        for (int i = 0; i < DataManager.Instance.currentSaveData.savedDays.Length; i++)
        {
            DataManager.Instance.currentSaveData.savedDays[i]++;
        }

        dayPassEvent?.Invoke();
        resetEvent?.Invoke();
    }

    public void SeasonChange()
    {
        switch (season)
        {
            case Season.Spring:
                season = Season.Summer;
                break;
            case Season.Summer:
                season = Season.Autumn;
                break;
            case Season.Autumn:
                TerrainScript.Instance.RevertSoil();
                season = Season.Winter;
                break;
            case Season.Winter:
                YearChange();
                season = Season.Spring;
                break;
        }
    }

    public void YearChange() { }


    void AddSecond()
    {

        // ghostSun.transform.Rotate(new Vector3(360 / (60 * 24 / minuteMultiplier), 0, 0));
        clockTime.y += minuteMultiplier;
        if (clockTime.y >= 60)
        {
            clockTime.x++;
            clockTime.y = 0;
            if (clockTime.x >= 24)
            {
                DayPass();
                clockTime.x = 0;
            }
        }

    }
}


public enum WeekDay { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
public enum Season { Spring, Summer, Autumn, Winter };