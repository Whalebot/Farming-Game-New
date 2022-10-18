using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StorageScript : Interactable
{
    public static bool inStorage;
    public GameObject storageWindow;
    [FoldoutGroup("Components")] public List<SellSlot> inventorySlots;
    [FoldoutGroup("Components")] public List<SellSlot> storageSlots;
    int sum;
    public DescriptionWindow descriptionWindow;

    public StorageData storageData;
    // Start is called before the first frame update
    void Start()
    {
        print(storageData);
        //SetupInventory();
        InputManager.Instance.eastInput += CloseStorage;
        InputManager.Instance.startInput += CloseStorage;
        LoadStorage();

    }

    public void Awake()
    {
        DataManager.Instance.loadDataEvent += UnloadObject;
        DataManager.Instance.saveDataEvent += SaveStorage;
    }

    public virtual void UnloadObject()
    {
        Destroy(gameObject);
    }


    public void SaveStorage()
    {
        if (storageData == null || storageData.ID == -1)
        {
            storageData = new StorageData();
            storageData.ID = DataManager.Instance.currentSaveData.storageData.Count;
        }

        storageData.storageInventory = new List<InventoryData>();
        storageData.sceneID = DataManager.Instance.sceneIndex;
        storageData.position = transform.position;
        storageData.rotation = transform.rotation;

        for (int i = 0; i < storageSlots.Count; i++)
        {
            InventoryData tempData = new InventoryData();
            if (storageSlots[i].itemSO != null)
            {
                tempData.itemID = storageSlots[i].itemSO.ID;
                tempData.quantity = storageSlots[i].quantity;
            }

            storageData.storageInventory.Add(tempData);
        }

        if (DataManager.Instance.currentSaveData.storageData.Count > storageData.ID && storageData.ID >= 0)
            DataManager.Instance.currentSaveData.storageData[storageData.ID] = storageData;
        else
            DataManager.Instance.currentSaveData.storageData.Add(storageData);
    }
    public void LoadStorage()
    {
        if (storageData != null && storageData.ID != -1)
        {
            for (int i = 0; i < storageSlots.Count; i++)
            {
                Item item = new Item(ItemDatabase.Instance.GetItemData(storageData.storageInventory[i].itemID), storageData.storageInventory[i].quantity, storageData.storageInventory[i].quality);
                storageSlots[i].UpdateSlot(item);
            }
        }
    }



    public override void Interact()
    {

        if (!inStorage)
        {
            OpenStorage();
        }
        else
        {
            CloseStorage();
        }
    }

    public void CloseStorage()
    {
        if (!inStorage) return;
        StartCoroutine("WaitFrame");
        storageWindow.SetActive(false);
    }
    public void OpenStorage()
    {
        SetupInventory();
        inStorage = true;
        storageWindow.SetActive(true);
        UIManager.Instance.SetActive(storageSlots[0].gameObject);
    }

    public bool HasItems()
    {
        bool temp = false;

        foreach (SellSlot slot in storageSlots)
        {
            if (slot.itemSO != null)
            {
                temp = true;
            }
        }

        return temp;
    }


    private void OnDisable()
    {
        InputManager.Instance.eastInput -= CloseStorage;
        InputManager.Instance.startInput -= CloseStorage;

        DataManager.Instance.saveDataEvent -= SaveStorage;
        DataManager.Instance.loadDataEvent -= UnloadObject;
        //  DataManager.Instance.loadItemProperties -= LoadStorage;
    }

    public void SetupInventory()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Item item = new Item(InventoryScript.Instance.inventorySlots[i].itemSO, InventoryScript.Instance.inventorySlots[i].quantity, InventoryScript.Instance.inventorySlots[i].quality);
        //    print(InventoryScript.Instance.inventorySlots[i].itemSO);
            inventorySlots[i].UpdateSlot(item);
            //sellSlots[i].UpdateSlot(null, 0);
        }
    }




    IEnumerator WaitFrame()
    {
        yield return new WaitForFixedUpdate();
        inStorage = false;
    }

    public void DescriptionWindow(ItemSO SO)
    {
        if (SO != null)
        {
            descriptionWindow.gameObject.SetActive(true);
            descriptionWindow.DisplayUI(SO);
        }
        else descriptionWindow.gameObject.SetActive(false);

    }

    public void MoveSingleFromInventory(SellSlot slot)
    {
        bool foundSO = false;


        int invIndex = inventorySlots.IndexOf(slot);
        int index = 0;

        for (int i = 0; i < storageSlots.Count; i++)
        {
            if (storageSlots[i].itemSO == slot.itemSO)
            {
                index = i;
                foundSO = true;
                break;
            }
        }

        if (foundSO)
        {
            Item item = new Item(storageSlots[index].itemSO, storageSlots[index].quantity + 1, storageSlots[index].quality);

            storageSlots[index].UpdateSlot(item);
            slot.quantity--;
            if (slot.quantity <= 0) slot.itemSO = null;

        }
        else
        {
            for (int i = 0; i < storageSlots.Count; i++)
            {
                if (storageSlots[i].itemSO == null)
                {
                    index = i;
                    break;
                }
            }

            Item item = new Item(slot.itemSO, 1, slot.quality);
            storageSlots[index].UpdateSlot(item);
            slot.quantity--;
            if (slot.quantity <= 0) slot.itemSO = null;

        }

        InventoryScript.Instance.DeleteItem(InventoryScript.Instance.inventorySlots[invIndex]);
        slot.UpdateSlot();
    }


    public void MoveFromInventory(SellSlot slot)
    {
        int invIndex = inventorySlots.IndexOf(slot);
        int index = 0;
        bool foundSO = false;

        for (int i = 0; i < storageSlots.Count; i++)
        {
            if (storageSlots[i].itemSO == slot.itemSO)
            {
                index = i;
                foundSO = true;
                break;
            }
        }

        if (foundSO)
        {
            Item item = new Item(storageSlots[index].itemSO, storageSlots[index].quantity + slot.quantity, storageSlots[index].quality + slot.quality);

            storageSlots[index].UpdateSlot(item);
        }
        else
        {
            for (int i = 0; i < storageSlots.Count; i++)
            {
                if (storageSlots[i].itemSO == null)
                {
                    index = i;
                    break;
                }
            }

            storageSlots[index].UpdateSlot(slot.item);
        }

        InventoryScript.Instance.DeleteSlot(InventoryScript.Instance.inventorySlots[invIndex]);
        slot.UpdateSlot(null);
    }


    [Button]
    public void RemoveAllFromShop()
    {
        foreach (SellSlot slot in storageSlots)
        {
            if (slot.itemSO != null)
                MoveFromShop(slot);
        }
    }

    public void MoveFromShop(SellSlot slot)
    {
        InventoryScript.Instance.PickupItem(slot.item);

        slot.UpdateSlot(null);

        SetupInventory();
    }

    public void MoveSingleFromShop(SellSlot slot)
    {
        InventoryScript.Instance.PickupItem(slot.item);
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateSlot();

        SetupInventory();
    }
}
