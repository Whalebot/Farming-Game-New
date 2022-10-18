using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IngredientSlot : MonoBehaviour
{
    Button thisButton;

    public int quantity;

    public ItemSO itemSO;
    public Image itemSprite;
    public Color failColor;
    public TMP_Text quantityText;
    public TMP_Text inventoryQuantity;

    public CraftingScript crafting;
    public GameObject hoverSFX;
    public GameObject clickSFX;
    public bool selected = false;

    // Start is called before the first frame update
    private void Awake()
    {
    }


    void Start()
    {
        InputManager.Instance.westInput += WestButton;
        //InputManager.Instance.northInput += NorthButton;
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
        selected = true;
        crafting.DescriptionWindow(itemSO);
        Instantiate(hoverSFX);
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
                Instantiate(clickSFX);

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

            }
        }
    }


    private void OnEnable()
    {
        if (itemSO != null)
        {
            Item item = new Item(itemSO, quantity, 0);
            UpdateSlot(item);
        }
    }

    void ButtonClicked()
    {

        Instantiate(clickSFX);
        UpdateQuantity();
    }

    public void UpdateSlot(Item item)
    {

        if (item == null) { ResetSlot(); return; }
        if (item.SO == null) { ResetSlot(); return; }
        itemSO = item.SO;
        itemSprite.sprite = item.SO.sprite;
        quantity = item.quantity;
        itemSprite.gameObject.SetActive(true);

        quantityText.gameObject.SetActive(quantity > 0 && item.SO.quantityLimit > 1);
        quantityText.text = "" + quantity;
        UpdateQuantity();
        itemSprite.gameObject.SetActive(quantity > 0);

        if (InventoryScript.Instance.CheckQuantity(item.SO) < item.quantity)
        {
            itemSprite.color = failColor;
        }
        else itemSprite.color = Color.white;

        if (quantity <= 0) itemSO = null;
    }

    void UpdateQuantity() {
        inventoryQuantity.gameObject.SetActive(true);
        inventoryQuantity.text = "" + InventoryScript.Instance.CheckQuantity(itemSO);
    }

    public void ResetSlot()
    {
        itemSO = null;
        quantity = 0;
        itemSprite.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
        inventoryQuantity.gameObject.SetActive(false);
    }
}
