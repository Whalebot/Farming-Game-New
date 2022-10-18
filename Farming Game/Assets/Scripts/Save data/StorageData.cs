using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageData
{
    public int ID = -1;
    public int sceneID;
    public Vector3 position;
    public Quaternion rotation;
    public List<InventoryData> storageInventory;
}
