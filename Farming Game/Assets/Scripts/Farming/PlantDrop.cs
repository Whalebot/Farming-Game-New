using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDrop : MonoBehaviour
{
    PlantScript plant;
    Status status;

    void Awake()
    {
        plant = GetComponentInParent<PlantScript>();
        status = GetComponent<Status>();
        status.deathEvent += SpawnDrop;
    }

    void SpawnDrop()
    {
        Item item = new Item(plant.SO, plant.quantity, plant.quality);
        InventoryScript.Instance.PickupItem(item);

    }


}
