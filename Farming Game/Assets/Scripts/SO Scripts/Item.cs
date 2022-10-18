using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public Item(ItemSO itemSO, int i, int q)
    {
        SO = itemSO;
        quantity = i;
        quality = q;
    }
    public ItemSO SO;
    public int quantity = 1;
    public int quality = 10;
    public int age = 0;
}
