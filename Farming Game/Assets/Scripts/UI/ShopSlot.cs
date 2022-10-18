using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    Button thisButton;
    public int itemID;
    public Item item;
    public int price;
    public int quantity;
    public int quality;

    public ItemSO itemSO;
    public Image itemSprite;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text quantityText;
    InventoryScript inventory;
    ShopScript shop;

   // public GameObject hoverSFX;
    //public GameObject clickSFX;
    public bool selected = false;
    // Start is called before the first frame update
    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryScript>();
        shop = GetComponentInParent<ShopScript>();
    }


    void Start()
    {

        InputManager.Instance.westInput += WestButton;
        thisButton = GetComponent<Button>();

        thisButton.onClick.AddListener(ButtonClicked);
        if (item.SO == null)
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
        shop.SetActive(this);
        shop.DescriptionWindow(item);
        selected = true;
       // Instantiate(hoverSFX);
    }
    public void Deselect()
    {
        selected = false;
    }

    void WestButton()
    {
        //if (GameManager.inventoryMenuOpen)
        //{

        //    if (selected)
        //    {
        //        Instantiate(clickSFX);
        //    }
        //}
    }


    private void OnEnable()
    {
        if (item.SO != null)
        {
            // item.SO = itemSO;
            UpdateSlot(item);

        }
    }

    private void OnValidate()
    {
        if (item.SO != null)
        {
            //item.SO = itemSO;
            UpdateSlot(item);

        }
    }

    void ButtonClicked()
    {
        //  if (InputManager.buttonUp)
        if (GameManager.Instance.Money >= price)
        {
           // Instantiate(clickSFX);

            bool notFull = inventory.PickupItem(item);
            if (notFull) GameManager.Instance.Money -= price;
        }
        else
        {

        }
    }


    public void UpdateSlot(Item temp)
    {
        if (temp == null) { ResetSlot(); return; }
        if (temp.SO == null) { ResetSlot(); return; }
        ItemSO SO = temp.SO;



        // itemSO = SO;
        itemSprite.sprite = SO.sprite;
        quantity = 1;
        quality = temp.quality;
        quantityText.gameObject.SetActive(true);
        itemSprite.gameObject.SetActive(true);
        //priceText.text = SO.baseValue;

        price = SO.baseValue + (SO.baseValue * item.quality / 100);
        priceText.text = "" + price;
        nameText.text = "" + SO.title;

        //item.SO = itemSO;
        //item.quantity = quantity;
        //item.quality = quality;

        //quantityText.gameObject.SetActive(quantity > 0 && itemSO.quantityLimit > 1);
        quantityText.text = "" + quality;
        //itemSprite.gameObject.SetActive(quantity > 0);
        // if (quantity <= 0) itemSO = null;
    }

    public void ResetSlot()
    {
        quantity = 0;
        itemSprite.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
    }
}
