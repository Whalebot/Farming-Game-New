using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : Interactable
{
    public Item item;
    public int quantity;
    public int quality = 10;
    public ItemSO SO;
    public bool picked;
    public bool colliding;
    public GameObject pickupFX;
    protected Collider lastCol;
    protected DataManager dataManager;
    protected ItemData itemData;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        SetupComponent();
    }
    private void OnDisable()
    {
        ResetComponent();
    }

    public virtual void SetupComponent()
    {
        dataManager = DataManager.Instance;
        //       SendItemData();
        dataManager.saveItemEvent += SaveItemData;
        dataManager.loadItemEvent += UnloadObject;
        if (quantity == 0) quantity = 1;
        item = new Item(SO, quantity, quality);
    }
    public virtual void ResetComponent()
    {
        dataManager.saveItemEvent -= SaveItemData;
        dataManager.loadItemEvent -= UnloadObject;
    }

    protected void SaveItemData()
    {
        itemData = new ItemData();
        itemData.sceneID = dataManager.sceneIndex;
        itemData.itemID = SO.ID;
        itemData.position = transform.position;
        itemData.rotation = transform.rotation;
        SendItemData();
    }

    void SendItemData()
    {
        //if(DataManager.isHouse) dataManager.currentSaveData.houseData.Add(itemData);
        //else
        dataManager.currentSaveData.itemData.Add(itemData);
    }


    public virtual void UnloadObject()
    {
        Destroy(gameObject);
    }


    public ItemSO PickedItem()
    {
        return SO;
    }

    public override void Interact()
    {
        base.Interact();
        Picked();
    }

    public void Picked()
    {
        //  Instantiate(pickupFX, transform.position, transform.rotation);
        InventoryScript.Instance.PickupItem(item);

        Destroy(gameObject);
    }
}
