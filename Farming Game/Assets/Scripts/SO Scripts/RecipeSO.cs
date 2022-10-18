using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/Recipe", order = 1)]
public class RecipeSO : ScriptableObject
{
    public ItemSO item;
    public Item[] ingredients;
}
