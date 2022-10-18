using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public EventSystem eventSystem;
    public Transform descriptionWindowPanel;
    public GameObject descriptionWindow;
    public GameObject inventoryFull;
    public InventoryScript inventory;
  //  public TextAnimatorPlayer moneyPlayer;

    public GameObject statusUI;
    public GameObject timeUI;

    public DescriptionWindow usedItemText;
    public DescriptionWindow foundItemWindow;

    [TabGroup("UI")] public Image equipImage;
    [TabGroup("UI")] public Image activeImage;
    [TabGroup("UI")] public TextMeshProUGUI activeQuantity;
    [TabGroup("UI")] public StatUpdateUI itemStatUI;
    [TabGroup("UI")] public TextMeshProUGUI indicatorText;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text weekDayText;
    [SerializeField] private Image seasonImage;
    [SerializeField] Sprite springSprite, summerSprite, autumnSprite, winterSprite;


    public static bool prioritizeUI;
    bool openWindow;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        InputManager.Instance.southInput += CloseWindow;
        InputManager.Instance.westInput += CloseWindow;
        UpdateMoney();
    }
    void ExecuteFrame() {
        if (GameManager.shopOpen || GameManager.menuOpen)
        {
            statusUI.SetActive(false);
            //  timeUI.SetActive(false);
        }
        else
        {
            statusUI.SetActive(true);
            //   timeUI.SetActive(true);
        }

        UpdateUI();

        if (inventory.mainHand.itemSO != null)
        {
            equipImage.gameObject.SetActive(true);
            equipImage.sprite = inventory.mainHand.itemSO.sprite;
        }
        else equipImage.gameObject.SetActive(false);


        if (inventory.activeItem.itemSO != null)
            activeImage.sprite = inventory.activeItem.itemSO.sprite;

        activeImage.gameObject.SetActive(inventory.activeItem.quantity > 0);
        activeQuantity.gameObject.SetActive(inventory.activeItem.quantity > 0);
        activeQuantity.text = "" + inventory.activeItem.quantity;


        if (!GameManager.inventoryMenuOpen
            //   && !TutorialManager.Instance.inTutorial
            )
        {
            inventory.useItemIndicator.SetActive(inventory.activeItem.quantity > 0);
            if (inventory.useItemIndicator.activeSelf) indicatorText.text = "" + inventory.activeItem.itemSO.itemUsage;
        }
        else inventory.useItemIndicator.SetActive(false);
    }

    void UpdateUI() {
        dateText.text = "" + TimeManager.Instance.date;
        timeText.text = TimeManager.Instance.clockTime.x.ToString("00") + " : " + TimeManager.Instance.clockTime.y.ToString("00");
        switch (TimeManager.Instance.weekDay)
        {
            case WeekDay.Monday:
                weekDayText.text = "Mon";
                break;
            case WeekDay.Tuesday:
                weekDayText.text = "Tue";
                break;
            case WeekDay.Wednesday:
                weekDayText.text = "Wed";

                break;
            case WeekDay.Thursday:
                weekDayText.text = "Thu";

                break;
            case WeekDay.Friday:
                weekDayText.text = "Fri";

                break;
            case WeekDay.Saturday:
                weekDayText.text = "Sat";
                break;
            case WeekDay.Sunday:
                weekDayText.text = "Sun";
                break;
        }

        switch (TimeManager.Instance.season)
        {
            case Season.Spring:
                seasonImage.sprite = springSprite;
                break;
            case Season.Summer:
                seasonImage.sprite = summerSprite;
                break;
            case Season.Autumn:
                seasonImage.sprite = autumnSprite;

                break;
            case Season.Winter:
                seasonImage.sprite = winterSprite;
                break;
        }


    }

    public void UpdateMoney() {
        
        //moneyPlayer.ShowText("" + DataManager.Instance.currentSaveData.profile.gold);
    }

    public void CloseWindow() {
        if (!openWindow) return;
      //  GameManager.Instance.ResumeGame();
        prioritizeUI = false;
        foundItemWindow.gameObject.SetActive(false);
        openWindow = false;
    }

    public void FoundItem(Item item) {
        prioritizeUI = true;
      //  GameManager.Instance.PauseGame();
        openWindow = true;
        foundItemWindow.gameObject.SetActive(true);
        foundItemWindow.DisplayUI(item);
    }

    public void DisplayPickedItem(ItemSO item) {
        GameObject GO = Instantiate(descriptionWindow, descriptionWindowPanel);
        GO.GetComponent<DescriptionWindow>().DisplayUI(item);
    }

    public void DisplayPickedItem(Item item)
    {
 
        GameObject GO = Instantiate(descriptionWindow, descriptionWindowPanel);
        GO.GetComponent<DescriptionWindow>().DisplayUI(item);
    }

    public void InventoryFull() {
        GameObject GO = Instantiate(inventoryFull, descriptionWindowPanel);
    }
    public void DisplayUsedItemEffect(Item item) {
        StopAllCoroutines();
        StartCoroutine("ActiveUsedItemWindow");
        usedItemText.DisplayUI(item);
    }

    IEnumerator ActiveUsedItemWindow() {
        usedItemText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2F);
        usedItemText.gameObject.SetActive(false);
    }

    public void SetActive(GameObject GO) {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(GO);

    }
}
