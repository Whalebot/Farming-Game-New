using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlantSO", menuName = "ScriptableObjects/Plants", order = 2)]
public class PlantSO : ScriptableObject
{
    public int ID;
    public GameObject prefab;

}
