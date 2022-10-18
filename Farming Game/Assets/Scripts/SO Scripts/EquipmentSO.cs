using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentSO", menuName = "ScriptableObjects/Equipment", order = 3)]
public class EquipmentSO : ItemSO
{
    public enum EquipmentType { MainHand, OffHand, Head, Top, Bottom };
    public EquipmentType equipmentType = new EquipmentType();

    public AnimatorOverrideController controller;
    public Moveset moveset;

    public int attack = 1;
    public float damageMultiplier = 1;
    public float attackSpeedMultiplier = 1;
}
