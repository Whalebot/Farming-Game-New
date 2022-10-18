using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SkillSO", menuName = "ScriptableObjects/Skill", order = 1)]
public class Skill : ScriptableObject
{
  
    public SkillType type;
    public string title;
    public Sprite sprite;
    public Move move;
    public Move move2;
    public Move move3;
    public int level;
    public int experience;
    [TextArea(15, 20)]
    public string description;
    [ShowIf("@type == SkillType.Passive")]
    public Stats stats;
}

[System.Serializable]
public class SkillExp
{
    public Skill skill;
    public int experience = 10;
}

public enum SkillType { Active, Passive }