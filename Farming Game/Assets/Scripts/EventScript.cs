using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class EventScript
{
    public enum EventType { Trigger, Stats, Relationships, Items, Dialogue , Requirement, Cost, Skill, Tutorial};
    public EventType eventType = new EventType();

    [HideIf("@eventType != EventType.Dialogue")]
    public DialogueSO eventDialogue;
    [HideIf("@eventType != EventType.Items")]
    public Item[] items;
    [HideIf("@eventType != EventType.Stats")]
    public StatType[] statTypes;
    [HideIf("@eventType != EventType.Trigger")]
    public string triggerName;
    [HideIf("@eventType != EventType.Skill")]
    public Skill skill;
    [HideIf("@eventType != EventType.Cost")]
    public int goldCost;
    [HideIf("@eventType != EventType.Cost")]
    public Item[] requiredItems;
    [HideIf("@eventType != EventType.Cost")]
    public DialogueSO reward;
    [HideIf("@eventType != EventType.Cost")]
    public DialogueSO fail;
    [HideIf("@eventType != EventType.Tutorial")]
    public TutorialSO tutorial;
}

[System.Serializable]
public class StatType {
    public enum Type { Health, Fatigue, MaxHealth, MaxStamina, Experience, Strength};
    public Type type = new Type();
    public int value;
}
