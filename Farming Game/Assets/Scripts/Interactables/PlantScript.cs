using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantScript : Interactable
{

    [TabGroup("Gameplay")] [Range(0, 10)] public int phase;
    [TabGroup("Gameplay")] [Min(0)] public int age;

    [TabGroup("Gameplay")] public ItemSO SO;
    [TabGroup("Gameplay")] [Min(0)] public int quantity;
    [TabGroup("Gameplay")] [Min(0)] public int quality = 10;
    [TabGroup("Gameplay")] [Min(0)] public int sunLevel = 0;

    [TabGroup("Gameplay")] public float idealWater;
    [TabGroup("Gameplay")] public bool growingYield;
    [TabGroup("Gameplay")] public bool noWater;
    [TabGroup("Gameplay")] public bool destroyOnHarvest;
    [TabGroup("Gameplay")] public float waterLevel;
    [TabGroup("Gameplay")] public int harvestTime;
    [TabGroup("Gameplay")] public GameObject harvestFX;
    [TabGroup("Gameplay")] public bool spring, summer, autumn, winter;
    [TabGroup("Visuals")] public GameObject[] phaseObjects;
    [TabGroup("Visuals")] public GameObject[] fruit;

    [TabGroup("Debug")] public int ID;
    [TabGroup("Debug")] public bool finished;
    [TabGroup("Debug")] public bool watered;
    [TabGroup("Debug")] public bool pickable;
    [TabGroup("Debug")] public bool picked;


    public int fruits;
    public int maxNumberOfFruits;
    public LayerMask plantMask;
    public float plantRadius = 0.5F;
    public int numberOfPlants;

    public float closestPlantDistance;
    Collider lastCol;
    TimeManager timeManager;
    InventoryScript inventoryScript;

    private void Awake()
    {


    }

    public void InitializePlant(int q)
    {
        quality = q;
        age = 0;
        sunLevel = 0;
    }

    [Button]
    void CheckNearbyPlants()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, plantRadius, plantMask);
        List<PlantScript> plants = new List<PlantScript>();
        foreach (var item in col)
        {
            PlantScript tempPlant = item.attachedRigidbody.GetComponent<PlantScript>();

            if (tempPlant != null && !plants.Contains(tempPlant) && tempPlant != this)
                plants.Add(tempPlant);
        }
        numberOfPlants = plants.Count;
        float dist = 100;
        foreach (var item in plants)
        {
            if (item == this) continue;
            float tempDist = Vector3.Distance(transform.position, item.transform.position);
            if (tempDist < dist)
            {

                dist = tempDist;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color c = Color.blue;
        c.a = 0.5F;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, plantRadius);
    }
    void Start()
    {
        DataManager.Instance.saveItemEvent += SavePlant;
        DataManager.Instance.loadItemEvent += UnloadObject;
        inventoryScript = InventoryScript.Instance;
        timeManager = TimeManager.Instance;
        timeManager.dayPassEvent += Grow;
        UpdateGO();
    }

    public void CheckSeason()
    {
        switch (TimeManager.Instance.season)
        {
            case Season.Spring: if (!spring) Destroy(gameObject); break;
            case Season.Summer: if (!summer) Destroy(gameObject); break;
            case Season.Autumn: if (!autumn) Destroy(gameObject); break;
            case Season.Winter: if (!winter) Destroy(gameObject); break;
        }
    }


    public float CheckPlantWater()
    {
        //float tempWater = TerrainScript.Instance.CheckWater(transform.position);
        float tempWater = 25;
        return tempWater / 4 * 100;
    }

    void SavePlant()
    {
        PlantData data = new PlantData();
        data.phase = phase;
        data.plantID = ID;
        data.quality = quality;
        data.waterLevel = waterLevel;
        data.sunLevel = sunLevel;
        data.position = transform.position;
        data.rotation = transform.rotation;
        DataManager.Instance.currentSaveData.plantData.Add(data);
    }

    void UnloadObject()
    {
        Destroy(gameObject);
    }

    void Update()
    {
    }

    private void OnDisable()
    {
        DataManager.Instance.saveItemEvent -= SavePlant;
        DataManager.Instance.loadItemEvent -= UnloadObject;
        timeManager.dayPassEvent -= Grow;
    }


    public override void Interact()
    {
        base.Interact();
        if (type == Type.None) return;
        Harvest();
    }

    public void Harvest()
    {
        if (picked) return;
        picked = true;
        if (harvestFX != null)
            Instantiate(harvestFX, transform.position, transform.rotation);

        Item item = new Item(SO, fruits, quality);
        inventoryScript.PickupItem(item);
        phase = phaseObjects.Length;
        fruits = 0;

        if (destroyOnHarvest) Destroy(gameObject);
    }



    void Grow()
    {
        CheckSeason();
        age++;
        float tempWater = TerrainScript.Instance.CheckWater(transform.position);
        waterLevel += tempWater;
        quality = Mathf.Clamp(quality + sunLevel/10 + (int) waterLevel/10, 0, 100);

        if (growingYield)
        {
            quantity = 1 + (quality / 20);
        }
        if (tempWater >= idealWater || noWater)
        {
            phase++;
            if (phase > phaseObjects.Length)
            {
                fruits = Mathf.Clamp(fruits + 1, 0, fruit.Length);
            }

        }
        watered = false;
        UpdateGO();
    }

    private void OnValidate()
    {
        if (Application.isEditor)
        {
            UpdateVisuals();
            CheckNearbyPlants();
        }
    }

    void UpdateGO()
    {
        if (phase >= phaseObjects.Length + harvestTime)
        {
            finished = true;
            picked = false;
        }
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        foreach (GameObject GO in phaseObjects)
        {
            GO.SetActive(false);
        }
        if (phase >= phaseObjects.Length)
            phaseObjects[phaseObjects.Length - 1].SetActive(true);
        else
            phaseObjects[phase].SetActive(true);

        for (int i = 0; i < fruit.Length; i++)
        {
            fruit[i].SetActive(i < fruits);
        }
    }
}
