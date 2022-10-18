using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class InventorySlot : MonoBehaviour
{
    public Item item;
    Button thisButton;
    public int itemID;
    public int quantity;
    public int quality;
    public enum InventoryType { Default, Active, MainHand, QuickSlot, Hotkey };
    public InventoryType type = InventoryType.Default;

    public ItemSO itemSO;
    public Image itemSprite;

    public Move move;

    public TMP_Text quantityText;
    InventoryScript inventory;

    //public GameObject hoverSFX;
    //public GameObject clickSFX;
    public bool selected = false;
    public int ID;
    [HideIf("@type != InventoryType.Hotkey")] public InventorySlot quickslotTarget;
    [HideIf("@type != InventoryType.Hotkey")] public ItemSO quickslotSO;
    [HideIf("@type != InventoryType.Hotkey")] public int quickslotID;
    [HideIf("@type != InventoryType.Hotkey")] public GameObject quickslotHighlight;
    // Start is called before the first frame update
    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryScript>();

    }


    void Start()
    {
        InputManager.Instance.westInput += WestButton;
        InputManager.Instance.northInput += NorthButton;
        if (type == InventoryType.Hotkey)
        {
            InputManager.Instance.L2release += ClickQuickslot;
        }

        thisButton = GetComponent<Button>();



        thisButton.onClick.AddListener(ButtonClicked);
        if (itemSO == null)
        {
            quantityText.gameObject.SetActive(false);
            itemSprite.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHover()
    {
        if (quantity == 0) inventory.DescriptionWindow(null);
        else inventory.DescriptionWindow(item);
        selected = true;
        // Instantiate(hoverSFX);
    }
    public void Deselect()
    {
        selected = false;
    }

    void NorthButton()
    {
        if (GameManager.inventoryMenuOpen)
        {

            if (selected)
            {
                //Instantiate(clickSFX);
            }
        }
    }

    void WestButton()
    {
        if (GameManager.inventoryMenuOpen)
        {

            if (selected)
            {
                //Instantiate(clickSFX);
                //if (itemSO != null)
                //    inventory.UseItem(this);
            }
        }
    }


    private void OnEnable()
    {
        if (type == InventoryType.Hotkey)
        {
            quickslotSO = quickslotTarget.quickslotSO;
            quickslotID = inventory.FindSlot(quickslotSO);
            itemSO = quickslotSO;
            quantity = inventory.CheckQuantity(itemSO);
        }

        if (itemSO != null)
        {
            item.SO = itemSO;
            item.quantity = quantity;
            item.quality = quality;
            UpdateSlot(item);
        }
    }

    void ButtonClicked()
    {

        //Instantiate(clickSFX);
        if (type == InventoryType.Hotkey) return;
        if (type == InventoryType.QuickSlot)
        {
            if (inventory.highlightedSlot == null)
                inventory.HighlightQuickslot(this);
            else
            {

            }
            return;
        }

        if (inventory.highlightedSlot == null)
        {
            if (itemSO != null)
            {
                if (type == InventoryType.Active)
                {
                    inventory.RemoveActiveItem();
                }
                else if (type == InventoryType.MainHand)
                {
                    inventory.UnequipItem(this);
                }
                else
                {
                    if (itemSO.GetType() == typeof(EquipmentSO))
                        inventory.EquipItem(this);
                    else inventory.SetActiveItem(this);
                }
            }
        }
        else
        {
            inventory.AssignQuickslot(this);
        }

    }

    public void ClickQuickslot()
    {
        if (!selected || itemSO == null) return;


        if (itemSO.GetType() == typeof(EquipmentSO))
        {
            if (inventory.mainHand.itemSO == itemSO) return;
            inventory.EquipItem(inventory.inventorySlots[quickslotID]);
        }
        else
        {
            //inventory.SetActiveItem(quickslotTarget);  
            if (inventory.activeItem.itemSO == itemSO) return;
            inventory.SetActiveItem(inventory.inventorySlots[quickslotID]);
        }

    }

    public void UpdateSlot()
    {
        if (itemSO == null) { ResetSlot(); return; }

        ItemSO SO = itemSO;

        itemSO = SO;
        itemID = SO.ID;
        itemSprite.sprite = itemSO.sprite;
        itemSprite.gameObject.SetActive(true);

        item.SO = SO;
        item.quantity = quantity;
        item.quality = quality;

        quantityText.gameObject.SetActive(quantity > 0 && SO.quantityLimit > 1);
        quantityText.text = "" + quantity;
        itemSprite.gameObject.SetActive(quantity > 0);
        if (quantity <= 0) itemSO = null;
    }

    public void UpdateSlot(Item temp)
    {
        if (temp == null) { ResetSlot(); return; }
        if (temp.SO == null) { ResetSlot(); return; }
        ItemSO SO = temp.SO;

        itemSO = SO;
        itemID = SO.ID;
        itemSprite.sprite = itemSO.sprite;
        quantity = temp.quantity;
        quality = temp.quality;
        itemSprite.gameObject.SetActive(true);


        item.SO = SO;
        item.quantity = quantity;
        item.quality = quality;

        quantityText.gameObject.SetActive(quantity > 0 && SO.quantityLimit > 1);
        quantityText.text = "" + quantity;
        itemSprite.gameObject.SetActive(quantity > 0);
        if (quantity <= 0) itemSO = null;
    }

    public void ResetSlot()
    {
        itemSO = null;
        quantity = 0;
        quality = 0;

        item.SO = itemSO;
        item.quantity = quantity;
        item.quality = quality;

        itemSprite.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
    }
}
