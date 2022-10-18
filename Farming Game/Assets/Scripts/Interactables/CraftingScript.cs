using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CraftingScript : Interactable
{
    public RecipeSlot[] recipeSlots;
    public IngredientSlot[] ingredientSlots;
    public DescriptionWindow descriptionWindow;
    public int selectedItem;
    public int activeChildCount;
    public GameObject craftingWindow;
    public Transform shopWindow;



    public List<RecipeSlot> activeSlots;
    public ButtonSelectionController selectionController;


    void Start()
    {
        InputManager.Instance.northInput += CloseShop;
        InputManager.Instance.eastInput += CloseShop;
        InputManager.Instance.startInput += CloseShop;
        activeChildCount = GetChildrenInContainer();
    }
    public void OpenShop()
    {
        for (int i = 0; i < shopWindow.childCount; i++)
        {
            activeSlots.Add(shopWindow.GetChild(i).GetComponent<RecipeSlot>());
        }

        GameManager.shopOpen = true;
        craftingWindow.SetActive(true);
        UIManager.Instance.SetActive(recipeSlots[0].gameObject);

    }
    public void CloseShop()
    {
 
        if (!GameManager.shopOpen || !craftingWindow.activeInHierarchy) return;
        GameManager.shopOpen = false;
        selectedItem = 0;
        craftingWindow.SetActive(false);
    }

    public override void Interact()
    {
        base.Interact();
        //
        //    CloseShop();
        //else,
        if (!GameManager.shopOpen)
            OpenShop();
    }

    public void SetActive(RecipeSlot slot)
    {
        int newSlot = activeSlots.IndexOf(slot);
        if (selectedItem + 3 < newSlot)
        {
            //Move down +1
            selectedItem++;
            selectedItem = Mathf.Clamp(selectedItem, 0, activeSlots.Count - 1);
            selectionController.UpdateScroll(true);
        }
        else if (selectedItem > newSlot)
        {
            //Move Up -1
            selectedItem--;
            selectedItem = Mathf.Clamp(selectedItem, 0, activeSlots.Count - 1);
            selectionController.UpdateScroll(false);
        }
    }

    int GetChildrenInContainer()
    {
        int count = 0;
        foreach (Transform child in shopWindow.transform)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }



    public void ShowRecipe(RecipeSO recipe) {
        foreach (var item in ingredientSlots)
        {
            item.ResetSlot();
        }

        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            ingredientSlots[i].UpdateSlot(recipe.ingredients[i]);
        }
    }

    public void DescriptionWindow(ItemSO so)
    {

        if (so == null) descriptionWindow.gameObject.SetActive(false);
        else
        {
            descriptionWindow.gameObject.SetActive(true);
            descriptionWindow.DisplayUI(so);
        }
    }
}
