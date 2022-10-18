using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;


public class SellScript : MonoBehaviour
{
    [FoldoutGroup("Components")] public List<SellSlot> inventorySlots;
    [FoldoutGroup("Components")] public List<SellSlot> sellSlots;
    public TextMeshProUGUI moneyText;
    int sum;
    public DescriptionWindow descriptionWindow;
    // Start is called before the first frame update
    void Start()
    {

        //SetupInventory();

    }

    public bool HasItems()
    {
        bool temp = false;

        foreach (SellSlot slot in sellSlots)
        {
            if (slot.itemSO != null)
            {
                temp = true;
            }
        }

        return temp;
    }

    private void OnEnable()
    {
        if (InputManager.Instance.controlScheme == ControlScheme.MouseAndKeyboard)
        {
        }
        else
        {
            InputManager.Instance.northInput += SellAll;
        }




        SetupInventory();
        UpdateMoney();
        UIManager.Instance.SetActive(sellSlots[0].gameObject);
    }

    private void OnDisable()
    {
        if (InputManager.Instance.controlScheme == ControlScheme.MouseAndKeyboard)
        {
        }
        else
        {
            InputManager.Instance.northInput -= SellAll;
        }
    }

    public void SetupInventory()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].UpdateSlot(InventoryScript.Instance.inventorySlots[i].item);
            //sellSlots[i].UpdateSlot(null, 0);
        }
    }

    public void DescriptionWindow(Item item)
    {
        if (item.SO != null)
        {
            descriptionWindow.gameObject.SetActive(true);
            descriptionWindow.DisplayUI(item);
        }
        else descriptionWindow.gameObject.SetActive(false);

    }

    public void MoveSingleFromInventory(SellSlot slot)
    {
        bool foundSO = false;


        int invIndex = inventorySlots.IndexOf(slot);
        int index = 0;

        for (int i = 0; i < sellSlots.Count; i++)
        {
            if (sellSlots[i].itemSO == slot.itemSO)
            {
                index = i;
                foundSO = true;
                break;
            }
        }

        if (foundSO)
        {
            Item item = new Item(sellSlots[index].itemSO, sellSlots[index].quantity + 1, sellSlots[index].quality);
            sellSlots[index].UpdateSlot(item);
            slot.quantity--;
            if (slot.quantity <= 0) slot.itemSO = null;

        }
        else
        {
            for (int i = 0; i < sellSlots.Count; i++)
            {
                if (sellSlots[i].itemSO == null)
                {
                    index = i;
                    break;
                }
            }
            Item item = new Item(slot.itemSO, 1, slot.quality);
            sellSlots[index].UpdateSlot(item);
            slot.quantity--;
            if (slot.quantity <= 0) slot.itemSO = null;

        }

        InventoryScript.Instance.DeleteItem(InventoryScript.Instance.inventorySlots[invIndex]);
        slot.UpdateSlot();
        UpdateMoney();
    }


    public void MoveFromInventory(SellSlot slot)
    {
        int invIndex = inventorySlots.IndexOf(slot);
        int index = 0;
        bool foundSO = false;

        for (int i = 0; i < sellSlots.Count; i++)
        {
            if (sellSlots[i].itemSO == slot.itemSO)
            {
                index = i;
                foundSO = true;
                break;
            }
        }

        if (foundSO)
        {
            Item item = new Item(sellSlots[index].itemSO, sellSlots[index].quantity + slot.quantity, (sellSlots[index].quality + slot.quality)/2);
            sellSlots[index].UpdateSlot(item);
        }
        else
        {
            for (int i = 0; i < sellSlots.Count; i++)
            {
                if (sellSlots[i].itemSO == null)
                {
                    index = i;
                    break;
                }
            }
            Item item = new Item(slot.itemSO, slot.quantity,slot.quality);
            sellSlots[index].UpdateSlot(item);
        }

        InventoryScript.Instance.DeleteSlot(InventoryScript.Instance.inventorySlots[invIndex]);
        slot.UpdateSlot(null);
        UpdateMoney();
    }

    void UpdateMoney()
    {
        sum = 0;
        foreach (SellSlot slot in sellSlots)
        {
            if (slot.item.SO != null)
                sum += (slot.item.SO.baseValue + slot.item.SO.baseValue * (slot.item.quality/100)) * slot.item.quantity;
        }
        moneyText.text = sum + "G";
    }

    public void SellAll()
    {
        GameManager.Instance.Money += sum;

        foreach (SellSlot slot in sellSlots)
        {
            if (slot.itemSO != null)
            {
                if (slot.itemSO.sellEvent.triggerName != "")
                    GameManager.Instance.SetTrigger(slot.itemSO.sellEvent.triggerName);
                slot.UpdateSlot(null);
            }

        }

        UpdateMoney();
    }

    [Button]
    public void RemoveAllFromShop()
    {
        foreach (SellSlot slot in sellSlots)
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
        UpdateMoney();
    }

    public void MoveSingleFromShop(SellSlot slot)
    {
        Item item = new Item(slot.itemSO, 1, slot.quality);
        InventoryScript.Instance.PickupItem(item);
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateSlot();

        SetupInventory();
        UpdateMoney();
    }
}
