using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public string title;
    public int quantityLimit;
    public int ageLimit = 0;
    public int baseValue;
    public ItemType type;
     public bool found;


    [TextArea(15, 20)]
    public string description;
    public GameObject gameObject;

    public enum ItemUsage { Unusable, Plant, Consume, Place };

    public ItemUsage itemUsage = new ItemUsage();
    [HideIf("@itemUsage != ItemUsage.Plant && itemUsage != ItemUsage.Place")]
    public GameObject plant;
    [HideIf("@itemUsage != ItemUsage.Plant")]
    public bool noSoil;
    [HideIf("@itemUsage != ItemUsage.Consume")]
    public EventScript eventScript;
    [HideIf("@itemUsage != ItemUsage.Consume")]
    public EventScript[] firstTimeEvents;
    [HideIf("@itemUsage != ItemUsage.Consume")] public bool used;
    public EventScript sellEvent;
}

public enum ItemType
{
    Equip,
    Ore,
    Crop,
    Seed,
    Furniture
}