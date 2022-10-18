using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class SellSlot : MonoBehaviour
{
    StorageScript storageScript;
    SellScript sellScript;


    Button thisButton;

    public int quantity;
    public enum SellType { Inventory, Shop, Storage, StorageInventory };
    public SellType type;
    public Item item;
    public int quality;
    public ItemSO itemSO;
    public Image itemSprite;

    public TMP_Text quantityText;

    //public GameObject hoverSFX;
    //public GameObject clickSFX;
    public bool selected = false;

    // Start is called before the first frame update
    private void Awake()
    {
        storageScript = GetComponentInParent<StorageScript>();
        sellScript = GetComponentInParent<SellScript>();
    }


    void Start()
    {
 

        //InputManager.Instance.northInput += NorthButton;
        thisButton = GetComponent<Button>();

        thisButton.onClick.AddListener(ButtonClicked);
        if (itemSO == null)
        {
            quantityText.gameObject.SetActive(false);
            itemSprite.gameObject.SetActive(false);
        }

    }



    void Gamepad()
    {
        InputManager.Instance.westInput += WestButton;
        InputManager.Instance.R3input -= WestButton;
    }

    void Keyboard()
    {
        InputManager.Instance.westInput -= WestButton;
        InputManager.Instance.R3input += WestButton;
    }

    public void OnHover()
    {
        selected = true;
        if (type == SellType.Storage || type == SellType.StorageInventory) { storageScript.DescriptionWindow(itemSO); }
        else
            sellScript.DescriptionWindow(item);
        //Instantiate(hoverSFX);
    }
    public void Deselect()
    {
        selected = false;
    }

    void NorthButton()
    {
        if (GameManager.shopOpen)
        {

            if (selected)
            {
              //  Instantiate(clickSFX);

            }
        }
    }

    void WestButton()
    {
        if (GameManager.shopOpen)
        {
            if (itemSO == null) return;
            if (selected)
            {
                //Instantiate(clickSFX);
                MoveAll();
            }
        }
    }

    void MoveOne()
    {
        if (type == SellType.Storage)
        {
            storageScript.MoveSingleFromShop(this);
        }
        else if (type == SellType.StorageInventory) {  storageScript.MoveSingleFromInventory(this); }
        else
        {
            if (type == SellType.Inventory)
                sellScript.MoveSingleFromInventory(this);
            else sellScript.MoveSingleFromShop(this);
        }

    }

    void MoveAll()
    {
        if (type == SellType.Storage)
        {
            storageScript.MoveFromShop(this);
        }
        else if (type == SellType.StorageInventory) { storageScript.MoveFromInventory(this); }
        else
        {
            if (type == SellType.Inventory)
                sellScript.MoveFromInventory(this);
            else sellScript.MoveFromShop(this);
        }
    }

    private void OnEnable()
    {
        if (itemSO != null)
        {
            UpdateSlot(item);
        }


        if (InputManager.Instance.controlScheme == ControlScheme.MouseAndKeyboard)
        {
            InputManager.Instance.northInput += WestButton;
        }
        else
        {
            InputManager.Instance.westInput += WestButton;
        }


    }

    private void OnDisable()
    {
        if (InputManager.Instance.controlScheme == ControlScheme.MouseAndKeyboard)
        {
            InputManager.Instance.northInput -= WestButton;
        }
        else
        {
            InputManager.Instance.westInput -= WestButton;
        }
    }

    void ButtonClicked()
    {

      //  Instantiate(clickSFX);

        if (itemSO != null)
        {
            MoveOne();
        }
    }

    public void UpdateSlot(Item temp)
    {
        if (temp == null) { ResetSlot(); return; }
        if (temp.SO == null) { ResetSlot(); return; }
        ItemSO SO = temp.SO;


        itemSO = SO;
        itemSprite.sprite = itemSO.sprite;
        quantity = temp.quantity;
        quality = temp.quality;

        itemSprite.gameObject.SetActive(true);


        item.SO = itemSO;
        item.quantity = quantity;
        item.quality = quality;

        quantityText.gameObject.SetActive(quantity > 0 && SO.quantityLimit > 1);
        quantityText.text = "" + quantity;
        itemSprite.gameObject.SetActive(quantity > 0);
        if (quantity <= 0) itemSO = null;
    }

    public void UpdateSlot()
    {
        if (itemSO == null) { ResetSlot(); return; }
        itemSprite.sprite = itemSO.sprite;
        itemSprite.gameObject.SetActive(true);

        quantityText.gameObject.SetActive(quantity > 0 && itemSO.quantityLimit > 1);
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
