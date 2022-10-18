using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile 
{
    public int gold = 100;
    public Season season;
    public Vector2 time;
    public int date;
    public Stats stats;
    public Stats currentStats;

}
