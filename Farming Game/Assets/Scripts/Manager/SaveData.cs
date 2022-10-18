using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public PlayerProfile profile;
    public string saveName;
    public SettingsData settings;

    public int[] savedDays;
    public int[] activeQuickslots;
    public int[] activeSkills;

    public InventoryData active;
    public InventoryData mainHand;
   public InventoryData offHand;
   public List<InventoryData> inventoryData;
   public List<StorageData> storageData;

    public GameTriggers triggers;
    public int npcStep;
    public List<bool> sceneLoaded;
    public List<SkillData> skillData;


    public List<ItemData> itemData;
    public List<PlantData> plantData;

    public List<float> terrainMap1;
    public List<float> terrainMap2;
    public List<float> terrainMap3;
    public List<float> terrainMap4;
    public List<float> terrainMap5;

}

[System.Serializable]
public class SceneData {
    public List<ItemData> itemData;
    public List<PlantData> plantData;
}
