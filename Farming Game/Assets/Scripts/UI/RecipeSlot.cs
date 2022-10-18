using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class RecipeSlot : MonoBehaviour
{
    Button thisButton;
    public int price;

    public RecipeSO recipe;

    public Image itemSprite;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text quantityText;
    InventoryScript inventory;
    CraftingScript crafting;

    public GameObject hoverSFX;
    public GameObject clickSFX;
    public bool selected = false;
    // Start is called before the first frame update
    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryScript>();
        crafting = GetComponentInParent<CraftingScript>();
    }


    void Start()
    {

        InputManager.Instance.westInput += WestButton;
        thisButton = GetComponent<Button>();

        thisButton.onClick.AddListener(ButtonClicked);
        if (recipe == null)
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
        crafting.SetActive(this);
        crafting.ShowRecipe(recipe);
        crafting.DescriptionWindow(recipe.item);
        selected = true;
        Instantiate(hoverSFX);
    }
    public void Deselect()
    {
        selected = false;
    }

    void WestButton()
    {
    }


    private void OnEnable()
    {
        if (recipe != null)
        {
            UpdateSlot(recipe);

        }
    }

    void ButtonClicked()
    {
        //  if (InputManager.buttonUp)
        // if (GameManager.Instance.Money >= price)
        {
            Instantiate(clickSFX);
            bool hasItems = InventoryScript.Instance.CheckForItems(recipe.ingredients);

            //bool notFull = inventory.PickupItem(itemSO, quantity);
            if (hasItems)
            {
                int qualitySum = 0;
                //for (int i = 0; i < recipe.ingredients.Length; i++)
                //{
                //    qualitySum += recipe.
                //}
                Item item = new Item(recipe.item, 1, qualitySum);
                if (InventoryScript.Instance.PickupItem(item))
                    InventoryScript.Instance.RemoveItems(recipe.ingredients);

                crafting.ShowRecipe(recipe);
            }
        }

    }

    private void OnValidate()
    {
        UpdateSlot(recipe);
    }

    public void UpdateSlot(RecipeSO recipe)
    {
        ItemSO SO = recipe.item;
        itemSprite.gameObject.SetActive(true);
        //priceText.text = SO.baseValue;
        itemSprite.sprite = SO.sprite;
        //price = SO.baseValue;
        //priceText.text = "" + SO.baseValue;
        if (InventoryScript.Instance != null)
            quantityText.text = "" + InventoryScript.Instance.CheckQuantity(recipe.item);
        nameText.text = "" + recipe.item.title;

    }

    public void ResetSlot()
    {
        itemSprite.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
    }
}
