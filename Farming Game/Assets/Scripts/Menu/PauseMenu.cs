using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class PauseMenu : MonoBehaviour
{

    public enum MenuTab { Inventory, Status, Calendar, Settings }
    [EnumToggleButtons] public MenuTab tab;

    [Header("Tabs")]
    [FoldoutGroup("Components")] public GameObject inventoryTab;
    [FoldoutGroup("Components")] public GameObject statusTab;
    [FoldoutGroup("Components")] public GameObject calendarTab;
    [FoldoutGroup("Components")] public GameObject settingsTab;

    [Header("Default Selection")]
    [FoldoutGroup("Components")] public GameObject statusDefault;
    [FoldoutGroup("Components")] public GameObject calendarDefault;
    [FoldoutGroup("Components")] public GameObject settingsDefault;
    [FoldoutGroup("Components")] public GameObject menu;

    public bool menuOpen;

    public TextMeshProUGUI titleTabText;
    public TextMeshProUGUI leftTabText;
    public TextMeshProUGUI rightTabText;

    public Slider masterSlider;
    public Slider camXSlider;
    public Slider camYSlider;


    public TextMeshProUGUI masterText;
    public TextMeshProUGUI camXText;
    public TextMeshProUGUI camYText;

    // Start is called before the first frame update
    void Awake()
    {

    }

    private void OnValidate()
    {
        menu.SetActive(menuOpen);
        UpdateMenuTab();
    }

    private void Start()
    {
        InputManager.Instance.R1input += SwitchTabRight;
        InputManager.Instance.L1input += SwitchTabLeft;

        InputManager.Instance.startInput += ToggleMenu;
        InputManager.Instance.eastInput += CloseMenu;
        InputManager.Instance.northInput += CloseMenu;
    }
    void CloseMenu()
    {
        if (!menuOpen) return;
        menuOpen = false;
        menu.SetActive(false);
        StartCoroutine("WaitFrame");
    }
    IEnumerator WaitFrame()
    {
        yield return new WaitForFixedUpdate();
        GameManager.inventoryMenuOpen = false;
    }


    void ToggleMenu()
    {
        if (GameManager.shopOpen || StorageScript.inStorage) return;
        menuOpen = !menuOpen;
        //ResetQuickslot();
        menu.SetActive(menuOpen);
        GameManager.inventoryMenuOpen = menuOpen;
        UpdateMenuTab();

        //StartCoroutine("WaitFrame");
    }


    public void UpdateMenuTab()
    {
        if (!menuOpen) return;
        inventoryTab.SetActive(false);
        statusTab.SetActive(false);
        calendarTab.SetActive(false);
        settingsTab.SetActive(false);


        titleTabText.text = "" + tab;

        if (tab == MenuTab.Inventory)
        {
            inventoryTab.SetActive(true);
            if (Application.isPlaying)
                UIManager.Instance.SetActive(InventoryScript.Instance.inventorySlots[0].gameObject);
            leftTabText.text = "" + (MenuTab.Settings);
            rightTabText.text = "" + (tab + 1);
        }
        if (tab == MenuTab.Status)
        {

            statusTab.SetActive(true);
            if (Application.isPlaying)
                UIManager.Instance.SetActive(statusDefault);
            leftTabText.text = "" + (tab - 1);
            rightTabText.text = "" + (tab + 1);
        }
        if (tab == MenuTab.Calendar)
        {
            calendarTab.SetActive(true);
            if (Application.isPlaying)
                UIManager.Instance.SetActive(calendarDefault);
            leftTabText.text = "" + (tab - 1);
            rightTabText.text = "" + (tab + 1);
        }
        if (tab == MenuTab.Settings)
        {
            settingsTab.SetActive(true);
            if (Application.isPlaying)
                UIManager.Instance.SetActive(settingsDefault);
            leftTabText.text = "" + (tab - 1);
            rightTabText.text = "" + MenuTab.Inventory;
        }
    }


    public void SetTab(int i)
    {
        if (!menuOpen) return;
        tab = (MenuTab)i;
        UpdateMenuTab();
    }

    public void SwitchTabRight()
    {
        if (!menuOpen) return;
        tab = tab + 1;
        if ((int)tab > 3) tab = 0;
        UpdateMenuTab();
    }

    public void SwitchTabLeft()
    {
        if (!menuOpen) return;
        tab = tab - 1;
        if ((int)tab < 0) tab = (MenuTab)3;
        UpdateMenuTab();
    }



    void LoadSettings()
    {
        //camXSlider.value = DataManager.Instance.currentSaveData.settings.cameraX;
        //camYSlider.value = DataManager.Instance.currentSaveData.settings.cameraY;
        //masterSlider.value = DataManager.Instance.currentSaveData.settings.masterVolume;
    }

    void SaveSettings()
    {

        DataManager.Instance.currentSaveData.settings.cameraX = camXSlider.value;
        DataManager.Instance.currentSaveData.settings.cameraY = camYSlider.value;
        DataManager.Instance.currentSaveData.settings.masterVolume = masterSlider.value;
    }

    private void OnEnable()
    {
        //  DataManager.Instance.saveDataEvent += SaveSettings;
        //  DataManager.Instance.loadDataEvent += LoadSettings;
       // LoadSettings();
        //UpdateValue();

    }

    private void OnDisable()
    {
        //   DataManager.Instance.saveDataEvent -= SaveSettings;
        //   DataManager.Instance.loadDataEvent -= LoadSettings;
    }

    public void UpdateValue()
    {
        masterText.text = "" + masterSlider.value;
        camXText.text = "" + camXSlider.value;
        camYText.text = "" + camYSlider.value;

        SaveSettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DeleteData()
    {
        DataManager.Instance.ClearData();

        SceneManager.LoadScene(0);
    }
    public void TitleScreen()
    {
        SceneManager.LoadScene(4);
    }
    public void CloseGame()
    {
        Application.Quit();
    }

}
