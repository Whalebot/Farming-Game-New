using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    public int enemyCount;
    public bool encounterFinished;

    public static float aimOffset = 1F;
    public delegate void AIEvent();

    public AIEvent newWaveEvent;
    public AIEvent allEnemiesKilled;
    public GameObject clearSFX;
    public AIEvent noEnemiesRoom;

    public List<AI> allEnemies;
    public List<AI> activeEnemies;


    public bool combatEncounter;
    private void Awake()
    {

        Instance = this;
        allEnemies = new List<AI>();


    }
    // Start is called before the first frame update
    void Start()
    {

    }


    public void NewWave()
    {
        newWaveEvent?.Invoke();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (activeEnemies.Count > 0 && !combatEncounter) {
            combatEncounter = true;
            AudioManager.Instance.GoToState(AudioManager.AudioState.Battle);
        }
        else if (activeEnemies.Count <= 0 && combatEncounter)
        {
            combatEncounter = false;
            AudioManager.Instance.GoToState(AudioManager.AudioState.Default);
        }

        enemyCount = allEnemies.Count;
 
    }

    public void KillAllEnemies()
    {

        int count = allEnemies.Count;
        for (int i = 0; i < count; i++)
        {
            //print(count + " " + AllEnemies[i]);
            allEnemies[0].GetComponent<Status>().Health = 0;
            //Destroy(AllEnemies[0].gameObject);
        }

    }

    void EmptyRoom()
    {
        encounterFinished = true;
        noEnemiesRoom?.Invoke();
    }

    public void AllEnemiesKilled()
    {
        if (combatEncounter)
            Instantiate(clearSFX);
        encounterFinished = true;
        allEnemiesKilled?.Invoke();
        //Debug.Log("INVOKED");
    }
}
