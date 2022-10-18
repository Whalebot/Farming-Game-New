using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

   public AudioSource AS;
    public AudioClip defaultBGM;
    public AudioClip[] seasonalBGM;
    public AudioSource battleMusic;
    public AudioSource dayAmbience;
    public AudioSource nightAmbience;
    public float fadeSpeed;
    float ASstart;
    float battleStart;

    public enum AudioState
    {
        Default, Night, Battle, Boss
    }
    public AudioState audioState;


    private void Awake()
    {
        Instance = this;
        ASstart = AS.volume;
        battleStart = battleMusic.volume;

    }


    // Start is called before the first frame update
    void Start()
    {

        TimeManager.Instance.sleepStartEvent += FadeOutVolume;
        if (seasonalBGM.Length > 0)
            if (seasonalBGM[(int)TimeManager.Instance.season] != null)
            {
                AS.clip = seasonalBGM[(int)TimeManager.Instance.season];
                AS.Play();
            }
    }

    public void FadeOutVolume(AudioSource source)
    {
        StartCoroutine("ReduceVolume", source);
    }

    public void FadeOutVolume()
    {
        StartCoroutine("ReduceVolume", AS);
    }

    IEnumerator ReduceVolume(AudioSource source)
    {
        while (source.volume > 0)
        {
            source.volume -= fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeInVolume(AudioSource source)
    {
        StartCoroutine("IncreaseVolume", source);
    }

    public void FadeInVolume()
    {
        StartCoroutine("IncreaseVolume");
    }

    IEnumerator BattleMusic()
    {

        while (AS.volume > 0)
        {
            AS.volume -= fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        battleMusic.gameObject.SetActive(true);
        while (battleMusic.volume < battleStart)
        {
            battleMusic.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator NightMusic()
    {

        while (AS.volume > 0)
        {
            AS.volume -= fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
        nightAmbience.volume = 0;
        nightAmbience.gameObject.SetActive(true);
        while (nightAmbience.volume < 1)
        {
            nightAmbience.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator IncreaseVolume()
    {
        while (AS.volume < ASstart)
        {
            AS.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator IncreaseVolume(AudioSource source)
    {
        while (source.volume < 1)
        {
            source.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void GoToState(AudioState state)
    {
        if (audioState != AudioState.Boss)
        {
            audioState = state;

            switch (audioState)
            {
                case AudioState.Default:
                    StopAllCoroutines();
                    StartCoroutine("FadeOutVolume", battleMusic);
                    FadeInVolume();
                    break;
                case AudioState.Battle:
                    StopAllCoroutines();
                    FadeOutVolume();
                    FadeOutVolume(nightAmbience);
                    StartCoroutine("BattleMusic");
                    break;
                case AudioState.Night:
                    StopAllCoroutines();
                    StartCoroutine("NightMusic");
                    break;
                case AudioState.Boss:
                    //StopAllCoroutines();



                    break;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (audioState)
        {
            case AudioState.Default:
                if (TimeManager.Instance.clockTime.x > 17)
                {
                    GoToState(AudioState.Night);
                }
                break;
            case AudioState.Battle:

                break;
        }

    }
}
