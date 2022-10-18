using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeEvent", menuName = "ScriptableObjects/TimeEvent", order = 5)]
public class TimeEvent : ScriptableObject
{
    public Sprite icon;
    public string eventName;
    [TextArea(3, 10)]
    public string eventDescription;
    public Season season;
    public int date;

}
