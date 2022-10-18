using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameTriggers
{

    //NPC
    public int leciaTriggerStep = -1;
    public int vaillantTriggerStep = -1;
    public bool vaillantUnlock;

    public int leciaEvent1 = 0;
    public int vaillantEvent1 = 0;
    public int dragonKill = 0;
    public bool dragonQuest;

    //Unlocks
    public bool carrotUnlock;
    public bool cornUnlock;
    public bool tomatoUnlock;
    public bool eggplantUnlock;
    public bool houseUpgrade1;
    public bool houseUpgrade2;

    public int slimeKills;
    public int beetleKills;
    public int dragonKills;


    ////Skills
    public bool skillTutorial;
    public bool skillTutorial2;

    public bool farmingTutorial;
    public bool farmingTutorial2;
    //public bool water;
    //public bool earth;
    //public bool wind;
}
