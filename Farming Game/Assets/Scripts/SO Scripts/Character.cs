using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "ScriptableObjects/Character", order = 2)]
public class Character : ScriptableObject
{
    public string characterName;
    public string subTitle;
    public Stats stats;
    public List<Skill> skills;
}
