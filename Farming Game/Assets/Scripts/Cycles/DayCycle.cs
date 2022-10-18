using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;

public class DayCycle : MonoBehaviour
{
    public static DayCycle Instance { get; private set; }
    DayPreset preset;
    public float temperature;
    public List<Weather> forecast;
    public Weather weather;
    public Weather lastWeather;
    [InlineEditor] public DayPreset winterPreset;
    [InlineEditor] public DayPreset springPreset;
    [InlineEditor] public DayPreset summerPreset;
    [InlineEditor] public DayPreset autumnPreset;
    public DayPreset rainPreset;
    public Volume springVolume, summerVolume, autumnVolume, winterVolume;
    public Season season;
    public Season lastSeason;

    [EnumToggleButtons]
    public enum DayPhase { Dawn, Noon, Dusk, Night }

    [FoldoutGroup("Settings")] public Light sun;
    [FoldoutGroup("Settings")] public Light moon;
    [FoldoutGroup("Settings")] public float moonSize;
    [FoldoutGroup("Settings")] public AnimationCurve moonIntensity;

    public float smoothDamp;
    public float shadowSmooth;

    [Range(0, 23)]
    public int hour;
    [Range(0, 59)]
    public int minute;

    [Range(0F, 1F)] public float normalizedTime;
    [Range(0F, 1F)] public float normalizedDay;

    public Vector3 sunDir;
    public float sunRotVal;

    [FoldoutGroup("Settings")]
    public Material skyboxMat;

    [FoldoutGroup("Time definition")] public DayPhase dayPhase;
    [FoldoutGroup("Time definition")] public Volume dawnVolume, dayVolume, duskVolume, nightVolume;
    [FoldoutGroup("Time definition")] public int dawnTime = 6, dayTime = 12, duskTime = 18, nightTime = 0;



    [FoldoutGroup("Settings")] public Transform dayCycleContainer;

    [FoldoutGroup("Stars")] public AnimationCurve starIntensityCurve;
    Vector3 startRotation;



    [FoldoutGroup("Clouds")]
    [Range(0, 0.1F)]
    public float windStrength;



    [FoldoutGroup("Clouds")] public Vector2 windDirection;
    [FoldoutGroup("Clouds")] public Vector2 cloudOffset;

    [FoldoutGroup("Snow")] [Range(0, 1)] public float snowOpacity;
    [FoldoutGroup("Snow")] public GameObject snowVFX;
    [FoldoutGroup("Rain")] public GameObject leafVFX;
    [FoldoutGroup("Rain")] [Range(0, 1)] public float rainIntensity;
    [FoldoutGroup("Rain")] public GameObject rainVFX;

    [FoldoutGroup("Debug")] public float timeTarget;

    float zeroFloat7;

    [FoldoutGroup("Debug")] [SerializeField] float startSunIntensity = 1.2F, startMoonIntensity = 1F;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startRotation = sun.transform.rotation.eulerAngles;
        //weather = forecast[TimeManager.Instance.date - 1];
        season = TimeManager.Instance.season;
        ChangeSeason();
        //TimeManager.Instance.dayPassEvent += ChangeSeason;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hour = (int)TimeManager.Instance.clockTime.x;
        minute = (int)TimeManager.Instance.clockTime.y;

        UpdateVisuals();
    }

    private void OnValidate()
    {
        ChangeSeason();
        UpdateVisuals();
    }

    void ChangeSeason()
    {

        springVolume.weight = 0;
        summerVolume.weight = 0;
        autumnVolume.weight = 0;
        winterVolume.weight = 0;
        snowOpacity = 0;
        //leafVFX.SetActive(false);
        //snowVFX.SetActive(false);
        //rainVFX.SetActive(false);
        switch (season)
        {
            case Season.Spring:
                preset = springPreset;
                springVolume.weight = 1;
                // if (lastSeason != season)

                break;
            case Season.Summer:
                preset = summerPreset;
                summerVolume.weight = 1;
                break;
            case Season.Autumn:
                preset = autumnPreset;
                //   leafVFX.SetActive(true);
                autumnVolume.weight = 1;
                break;
            case Season.Winter:
                preset = winterPreset;
                winterVolume.weight = 1;

                //Snow stuff
                snowOpacity = 1;
                //snowVFX.SetActive(true);
                //if (lastSeason != season)

                break;
        }
        lastSeason = season;
        if (weather != lastWeather)
        {

            if (weather == Weather.Rainy)
            {
                preset = rainPreset;
                rainVFX.SetActive(true);
            }

            lastWeather = weather;
        }

    }


    void UpdateVisuals()
    {
        float temp;
        if ((minute + 60 * hour) / 1440F == 0) temp = 0;
        else temp = normalizedTime;
        normalizedTime = (minute + 60 * hour) / 1440F;

        normalizedDay = preset.timeCurve.Evaluate(normalizedTime);
        timeTarget = Mathf.LerpAngle(timeTarget, (normalizedDay + 0.5F) * 360, 0.2F);

        temperature = preset.temperatureCurve.Evaluate(normalizedDay);

        if (Mathf.Abs(dawnTime - hour) < Mathf.Abs(Mathf.Abs(dawnTime - hour) - 24))
            dawnVolume.weight = 1 - Mathf.Abs(dawnTime - hour - (minute / 60)) / 6;
        else dawnVolume.weight = 1 - Mathf.Abs(Mathf.Abs(dawnTime - hour - (minute / 60)) - 24) / 6;

        if (Mathf.Abs(dayTime - hour) < Mathf.Abs(Mathf.Abs(dayTime - hour) - 24))
            dayVolume.weight = 1 - Mathf.Abs(dayTime - hour - (minute / 60)) / 6;
        else dayVolume.weight = 1 - Mathf.Abs(Mathf.Abs(dayTime - hour - (minute / 60)) - 24) / 6;

        if (Mathf.Abs(duskTime - hour) < Mathf.Abs(Mathf.Abs(duskTime - hour - (minute / 60)) - 24))
            duskVolume.weight = 1 - Mathf.Abs(duskTime - hour - (minute / 60)) / 6;
        else duskVolume.weight = 1 - Mathf.Abs(Mathf.Abs(duskTime - hour - (minute / 60)) - 24) / 6;


        if (Mathf.Abs(nightTime - hour) < Mathf.Abs(Mathf.Abs(nightTime - hour - (minute / 60)) - 24))
            nightVolume.weight = 1 - Mathf.Abs(nightTime - hour - (minute / 60)) / 6;
        else nightVolume.weight = 1 - Mathf.Abs(Mathf.Abs(nightTime - hour - (minute / 60)) - 24) / 6;


        //PRESET STUFF
        //sun.color = preset.sunGradient.Evaluate(normalizedDay);
        //Sky color
        skyboxMat.SetColor("_SkyColor", preset.skyGradient.Evaluate(normalizedDay));
        skyboxMat.SetColor("_HorizonColor", preset.horizonGradient.Evaluate(normalizedDay));
        skyboxMat.SetColor("_GroundColor", preset.groundGradient.Evaluate(normalizedDay));

        skyboxMat.SetFloat("_SkyExponent", preset.skyExponentCurve.Evaluate(normalizedDay));

        RenderSettings.ambientSkyColor = preset.skyGradient.Evaluate(normalizedDay);
        RenderSettings.ambientEquatorColor = preset.horizonGradient.Evaluate(normalizedDay);
        RenderSettings.ambientGroundColor = preset.groundGradient.Evaluate(normalizedDay);

        //Sun & moon position
        sunDir = sun.transform.forward;
        sunRotVal = -timeTarget;
        skyboxMat.SetFloat("_SunRotVal", sunRotVal + 90);

        skyboxMat.SetFloat("_MoonIntensity", moonIntensity.Evaluate(normalizedDay));
        skyboxMat.SetVector("_SunDirection", sunDir);

        skyboxMat.SetFloat("_StarIntensity", starIntensityCurve.Evaluate(normalizedDay));

        windDirection = windDirection.normalized;
        cloudOffset += windDirection * windStrength;
        skyboxMat.SetVector("_CloudOffset", cloudOffset);

        RenderSettings.fogColor = preset.fogGradient.Evaluate(normalizedDay);
        RenderSettings.fogDensity = preset.fogDensity.Evaluate(normalizedDay);

        //snowMat.SetFloat("_SnowOpacity", snowOpacity);
        Shader.SetGlobalFloat("_SnowOpacity", snowOpacity);

        if (hour >= 18 || hour < 6)
        {
            sun.intensity = 0;


            if (sun.intensity <= 0)
            {
                sun.shadowStrength = 0;

            }
            else if (sun.intensity < 0.01) sun.intensity = 0;

            if (sun.shadowStrength <= 0)
                moon.intensity = preset.moonIntensity;
            else if (sun.shadowStrength < 0.01) sun.shadowStrength = 0;


            if (sun.shadowStrength <= 0)
            {
                moon.shadowStrength = 1;
            }
            else if (sun.shadowStrength < 0.01)
            {
                sun.shadowStrength = 0;
            }
        }
        else
        {
            moon.intensity = 0;


            if (moon.intensity <= 0)
            {
                moon.shadowStrength = 0;

            }
            else if (moon.intensity < 0.01) moon.intensity = 0;

            if (moon.shadowStrength <= 0)
                sun.intensity = preset.sunIntensity;
            else if (moon.shadowStrength < 0.01) moon.shadowStrength = 0;

            if (moon.shadowStrength <= 0)
            {
                sun.shadowStrength = 1;
            }
            else if (moon.shadowStrength < 0.01)
            {
                moon.shadowStrength = 0;
            }
        }

        dayCycleContainer.rotation = Quaternion.Euler(timeTarget, 0, 0);
    }

    IEnumerator TurnOffShadows()
    {
        while (sun.shadowStrength > 0)
        {


            yield return new WaitForFixedUpdate();
        }

    }
}
public enum Weather
{
    Sunny,
    Cloudy,
    Rainy
}