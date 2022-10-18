using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public bool clearedTutorial;
    public bool killedBoss;
    public int ambienceIteration;

    public bool vSync = true;
    public float masterVolume = 1;
    public float sfxVolume = 1;
    public float musicVolume = 1;
}