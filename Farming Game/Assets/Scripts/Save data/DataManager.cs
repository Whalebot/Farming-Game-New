using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;
using Sirenix.OdinInspector;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

public class DataManager : MonoBehaviour
{

    public static DataManager Instance { get; private set; }
    public bool autoLoad = false;
    public bool autoSave = false;
    public static bool isHouse;
    public static bool isHub;
    public Character character;
    public List<SaveData> saveData;
    public SaveData currentSaveData;


    public delegate void DataEvent();
    public DataEvent saveDataEvent;
    public DataEvent loadDataEvent;

    public DataEvent saveItemEvent;
    public DataEvent loadItemEvent;
    public DataEvent loadItemProperties;
    public int sceneIndex;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        saveData = new List<SaveData>();

        currentSaveData = new SaveData();
        currentSaveData.profile = new PlayerProfile();
        //currentSaveData.settings = new SettingsData();
        currentSaveData.sceneLoaded = new List<bool>();

        currentSaveData.itemData = new List<ItemData>();
        currentSaveData.plantData = new List<PlantData>();
        currentSaveData.inventoryData = new List<InventoryData>();
        currentSaveData.storageData = new List<StorageData>();
        currentSaveData.active = new InventoryData();
        currentSaveData.mainHand = new InventoryData();
        //currentSaveData.triggers = new GameTriggers();
        currentSaveData.skillData = new List<SkillData>();
        currentSaveData.activeSkills = new int[4];
        currentSaveData.activeQuickslots = new int[4];

        currentSaveData.savedDays = new int[SceneManager.sceneCountInBuildSettings];
        isHouse = SceneManager.GetActiveScene().name == "House";
        isHub = SceneManager.GetActiveScene().name == "Hub";
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            currentSaveData.sceneLoaded.Add(false);
        }

    }

    private void Start()
    {
        if (HasSaveData())
        {
            if (autoLoad)
            {
                LoadData();
            }
        }
        currentSaveData.sceneLoaded[sceneIndex] = true;
    }

    private void OnApplicationQuit()
    {
        if (autoSave) SaveData();
    }

    public bool HasSaveData()
    {
        return File.Exists(Application.persistentDataPath + "/saveData.json");
    }

    [MenuItem("Data/DeleteSaveData %3")]
    public static void DeleteSaveData()
    {
        File.Delete(Application.persistentDataPath + "/saveData.json");
    }

    [Button]
    public void ClearData()
    {
        File.Delete(Application.persistentDataPath + "/saveData.json");

        //TransitionManager.Instance.LoadHub();
    }
    [Button]
    public void LoadData()
    {
        currentSaveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/saveData.json"));
        loadDataEvent?.Invoke();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - currentSaveData.sceneLoaded.Count; i++)
        {
            currentSaveData.sceneLoaded.Add(true);
        }

        if (!currentSaveData.sceneLoaded[sceneIndex])
        {
            currentSaveData.sceneLoaded[sceneIndex] = true;
            return;
        }


        //for (int i = 0; i < currentSaveData.storageData.Count; i++)
        //{
        //    if (currentSaveData.storageData[i].sceneID == sceneIndex)
        //    {
        //        GameObject GO = Instantiate(itemDatabase.storage, currentSaveData.storageData[i].position, currentSaveData.storageData[i].rotation);
        //        StorageScript temp = GO.GetComponent<StorageScript>();
        //        temp.storageData = currentSaveData.storageData[i];
        //    }
        //}

        loadItemEvent?.Invoke();
        loadItemProperties?.Invoke();


        for (int i = 0; i < currentSaveData.itemData.Count; i++)
        {
            if (currentSaveData.itemData[i].sceneID == sceneIndex)

            {
                if (currentSaveData.itemData[i].itemID != 0)
                {
                    GameObject GO = Instantiate(ItemDatabase.Instance.GetItemData(currentSaveData.itemData[i].itemID).gameObject, currentSaveData.itemData[i].position, currentSaveData.itemData[i].rotation);
                    //ItemScript temp = GO.GetComponent<ItemScript>();
                    //temp.quality = currentSaveData.itemData[i].}
                }
            }

        }
        for (int i = 0; i < currentSaveData.plantData.Count; i++)
        {

            GameObject GO = Instantiate(ItemDatabase.Instance.GetPlant(currentSaveData.plantData[i].plantID), currentSaveData.plantData[i].position, currentSaveData.plantData[i].rotation);
            //Set growth phase
            PlantScript plant = GO.GetComponent<PlantScript>();
            plant.phase = currentSaveData.plantData[i].phase;
            plant.quality = currentSaveData.plantData[i].quality;
            plant.waterLevel = currentSaveData.plantData[i].waterLevel;
        }
    }

    [Button]
    public void SaveData()
    {
        print("Game saved");
        List<int> index = new List<int>();
        for (int i = 0; i < currentSaveData.itemData.Count; i++)
        {
            if (currentSaveData.itemData[i].sceneID == sceneIndex) index.Add(i);
        }
        for (int i = index.Count - 1; i >= 0; i--)
        {
            currentSaveData.itemData.RemoveAt(index[i]);
        }

        //if (SceneManager.GetActiveScene().name == "Hub")
        //{
        //    // currentSaveData.itemData.Clear();
        //    currentSaveData.plantData.Clear();

        //}
        saveItemEvent?.Invoke();

        saveDataEvent?.Invoke();


        string jsonData = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", jsonData);
    }


    public bool CheckTrigger(string n)
    {
        if (n == "") return false;

        //GameTriggers triggers = currentSaveData.triggers;
        // if (triggers == null) return false;

        //FieldInfo[] defInfo = triggers.GetType().GetFields();

        //for (int i = 0; i < defInfo.Length; i++)
        //{
        //    object obj = triggers;
        //    if (defInfo[i].GetValue(obj) is bool)
        //    {
        //        if (defInfo[i].Name.Contains(n))
        //        {

        //            return (bool)defInfo[i].GetValue(obj);
        //        }
        //    }
        //    else if (defInfo[i].GetValue(obj) is int)
        //    {
        //        if (defInfo[i].Name.Contains(n))
        //        {
        //            if ((int)defInfo[i].GetValue(obj) == 1)
        //                return true;
        //        }
        //    }
        //}

        return false;
    }

    public void SetTrigger(string n)
    {
        if (n == "") return;

        //GameTriggers triggers = currentSaveData.triggers;
        // if (triggers == null) return false;

        //    FieldInfo[] defInfo = triggers.GetType().GetFields();

        //    for (int i = 0; i < defInfo.Length; i++)
        //    {
        //        object obj = triggers;
        //        if (defInfo[i].GetValue(obj) is bool)
        //        {
        //            if (defInfo[i].Name.Contains(n))
        //            {

        //                defInfo[i].SetValue(obj, true);
        //            }
        //        }
        //        else if (defInfo[i].GetValue(obj) is int)
        //        {
        //            if (defInfo[i].Name.Contains(n))
        //            {
        //                defInfo[i].SetValue(obj, (int)defInfo[i].GetValue(obj) + 1);
        //            }
        //        }
        //    }
    }
}
