using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class ShopScript : Interactable
{
    public float shopMultiplier;
    public GameObject shopWindow;
    public GameObject buyWindow;
    public GameObject sellWindow;
    SellScript sellScript;
    public ShopSlot[] shopSlots;
    public DescriptionWindow descriptionWindow;
    public enum ShopMode { Buying, Selling }
    public ShopMode mode;
    // public TextAnimatorPlayer title;
    public int selectedItem;
    public int activeChildCount;
    public List<ShopSlot> activeShopSlots;
    public ButtonSelectionController selectionController;

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

    void SetupActiveShopSlots()
    {
        foreach (ShopSlot slot in GetComponentsInChildren<ShopSlot>(true))
        {
            if (slot.gameObject.activeSelf)
                activeShopSlots.Add(slot);

        }
    }

    public void SetActive(ShopSlot slot)
    {
        int newSlot = activeShopSlots.IndexOf(slot);
        if (selectedItem + 3 < newSlot)
        {
            //Move down +1
            selectedItem++;
            selectedItem = Mathf.Clamp(selectedItem, 0, activeShopSlots.Count - 1);
            selectionController.UpdateScroll(true);
        }
        else if (selectedItem > newSlot)
        {
            //Move Up -1
            selectedItem--;
            selectedItem = Mathf.Clamp(selectedItem, 0, activeShopSlots.Count - 1);
            selectionController.UpdateScroll(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupActiveShopSlots();
        InputManager.Instance.eastInput += CloseShop;
        InputManager.Instance.startInput += CloseShop;
        InputManager.Instance.L1input += OpenBuy;
        InputManager.Instance.R1input += OpenSell;
        activeChildCount = GetChildrenInContainer();
        sellScript = sellWindow.GetComponent<SellScript>();
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

    [Button]
    public void ToggleShop()
    {
        if (!GameManager.shopOpen) return;
        if (sellWindow.activeSelf) OpenBuy();
        else OpenSell();
    }

    public void OpenShop()
    {
        GameManager.shopOpen = true;
        shopWindow.SetActive(true);
        OpenBuy();
        // UIManager.Instance.SetActive(shopSlots[0].gameObject);
    }

    public void OpenBuy()
    {
        if (!GameManager.shopOpen) return;
        mode = ShopMode.Buying;
        buyWindow.SetActive(true);
        sellWindow.SetActive(false);
        UIManager.Instance.SetActive(shopSlots[0].gameObject);
    }

    public void OpenSell()
    {
        if (!GameManager.shopOpen) return;
        mode = ShopMode.Selling;
        buyWindow.SetActive(false);
        sellWindow.SetActive(true);
    }

    public void CloseShop()
    {
        if (!GameManager.shopOpen || !shopWindow.activeInHierarchy) return;
        if (mode == ShopMode.Selling)
        {
            if (sellScript.HasItems())
            {
                sellScript.RemoveAllFromShop();
                return;
            }
        }
        else
        {

        }


        selectedItem = 0;
        shopWindow.SetActive(false);
        StartCoroutine("WaitFrame");
    }



    IEnumerator WaitFrame()
    {
        yield return new WaitForFixedUpdate();
        GameManager.shopOpen = false;
    }

    public void DescriptionWindow(Item item)
    {

        if (item == null) descriptionWindow.gameObject.SetActive(false);
        else
        {
            descriptionWindow.gameObject.SetActive(true);
            descriptionWindow.DisplayUI(item);
        }
    }
}
