using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.IO;
using Sirenix.OdinInspector;



public class TerrainScript : MonoBehaviour
{
    public static TerrainScript Instance { get; private set; }
    public Terrain t;

    TerrainData terrainData;
    public int defaultSize;
    public int[] unfarmableLayers;
    public float addHeight;
    public DayCycle cycle;
    public bool checkOffset;
    public Vector3 offset;

    public Texture2D waterTexture;
    public Texture2D tillTexture;
    float[] strength;
    public int revertSoilChance;
    private void Awake()
    {
        Instance = this;
        DataManager.Instance.saveDataEvent += SaveTerrain;
        DataManager.Instance.loadDataEvent += LoadTerrain;
        TimeManager.Instance.resetEvent += ResolveWeather;
    }

    private void Start()
    {
        // if (cycle.weather == Weather.Rainy) WaterTerrain();
        Initialize();
    }


    private void OnDisable()
    {
        DataManager.Instance.saveDataEvent -= SaveTerrain;
        DataManager.Instance.loadDataEvent -= LoadTerrain;
        TimeManager.Instance.resetEvent -= ResolveWeather;
    }

    void ResolveWeather() {
        switch (DayCycle.Instance.weather)
        {
            case Weather.Sunny:
                DryTerrain();
                break;
            case Weather.Cloudy:
                break;
            case Weather.Rainy:
                RevertSoil();
                break;
            default:
                break;
        }
    }

    float GetStrength(int x, int y)
    {
        x = Mathf.Clamp(x, 0, defaultSize - 1);
        y = Mathf.Clamp(y, 0, defaultSize - 1);

        return strength[y * defaultSize + x];
    }

    [Button]
    public void Initialize()
    {
        if (t == null)
            t = GetComponent<Terrain>();

        terrainData = t.terrainData;

        offset = new Vector3(defaultSize / 52.08F, 0, defaultSize / 52.08F);


        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //Normalize Coordinates
                float x01 = (float)x / terrainData.alphamapWidth;
                float y01 = (float)y / terrainData.alphamapHeight;

                //Get Height at point
                float height = terrainData.GetHeight(Mathf.RoundToInt(y01 * terrainData.heightmapResolution), Mathf.RoundToInt(x01 * terrainData.heightmapResolution));

                //Get Steepness at point
                float steepness = terrainData.GetSteepness(y01, x01);
            }


        }
        //size = brushTexture.height;

        strength = new float[defaultSize * defaultSize];

        for (int i = 0; i < defaultSize; i++)
        {
            for (int j = 0; j < defaultSize; j++)
            {
                strength[i * defaultSize + j] = waterTexture.GetPixelBilinear((float)j / defaultSize, (float)i / defaultSize).a;
            }
        }


    }

    [Button]
    public void TillBrush(Vector3 worldPos, float opacity = 1)
    {
        Vector3 offsetPos = worldPos - offset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        //Debug.Log($"{mapX} {mapZ}");
        if (mapX < 0 || mapZ < 0 || mapX > t.terrainData.heightmapResolution - defaultSize || mapZ > t.terrainData.heightmapResolution - defaultSize)
        {
            Debug.Log($"Out of bounds");
            return;
        }

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, defaultSize, defaultSize);

        for (int x = 0; x < defaultSize; x++)
        {
            for (int y = 0; y < defaultSize; y++)
            {
                float max = Mathf.Clamp(GetStrength(x, y) * opacity, 0, splatmapData[x, y, 0]);


                splatmapData[x, y, 0] = Mathf.Clamp01(splatmapData[x, y, 0] - max);
                splatmapData[x, y, 1] = Mathf.Clamp01(splatmapData[x, y, 1] + max);

                //Debug.Log($"{splatmapData[y, x, 0] + splatmapData[y, x, 1] + splatmapData[y, x, 2]} {splatmapData[y, x, 0]} {splatmapData[y, x, 1]} {splatmapData[y, x, 2]}");
            }
        }

        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }

    [Button]
    public void WaterBrush(Vector3 worldPos, float opacity = 1)
    {
        Vector3 offsetPos = worldPos - offset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        if (mapX < 0 || mapZ < 0 || mapX > t.terrainData.heightmapResolution - defaultSize || mapZ > t.terrainData.heightmapResolution - defaultSize)
        {
            Debug.Log($"Out of bounds");
            return;
        }

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, defaultSize, defaultSize);

        for (int x = 0; x < defaultSize; x++)
        {
            for (int y = 0; y < defaultSize; y++)
            {
                float max = Mathf.Clamp(GetStrength(x, y) * opacity, 0, splatmapData[x, y, 1]);

                splatmapData[x, y, 1] = Mathf.Clamp01(splatmapData[x, y, 1] - max);
                splatmapData[x, y, 2] = Mathf.Clamp01(splatmapData[x, y, 2] + max);

            }
        }

        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }


    [Button]
    public void WinterGround()
    {
        SwapTexture(0, 5);
        SwapTexture(1, 6);
    }
    [Button]
    public void NormalGround()
    {
        SwapTexture(5, 0);
        SwapTexture(6, 1);
    }

    void SwapTexture(int start, int end)
    {

        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                float temp = maps[x, y, start];
                float temp2 = maps[x, y, end];
                float sum = temp + temp2;

                maps[x, y, start] = 0;
                maps[x, y, end] = sum;

            }

        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    // Blend the two terrain textures according to the steepness of
    // the slope at each point.
    void Paint(int terrainLayer)
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                for (int i = 0; i < maps.GetLength(2); i++)
                {
                    maps[x, y, i] = 0;
                }
                maps[x, y, terrainLayer] = 1;
            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    void AddAlphaNoise(Terrain t, float noiseScale)
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                float a0 = maps[x, y, 0];
                float a1 = maps[x, y, 1];

                a0 += Random.value * noiseScale;
                a1 += Random.value * noiseScale;

                float total = a0 + a1;

                maps[x, y, 0] = a0 / total;
                maps[x, y, 1] = a1 / total;
            }
        }

        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    public void TillTerrain(Vector3 worldPos)
    {
        Vector3 offsetPos = worldPos - offset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, defaultSize, defaultSize);

        for (var y = 0; y < defaultSize; y++)
        {
            for (var x = 0; x < defaultSize; x++)
            {
                //Circle  if ((x - defaultSize/2) * (x - defaultSize / 2) + (y - defaultSize / 2) * (y - defaultSize / 2) <= defaultSize / 2 * defaultSize / 2)
                {
                    foreach (int layer in unfarmableLayers)
                    {
                        if (splatmapData[x, y, layer] > 0.5F) return;
                    }
                    if (cycle.weather == Weather.Rainy)
                    {
                        splatmapData[x, y, 0] = 0;
                        splatmapData[x, y, 1] = 0;
                        splatmapData[x, y, 2] = 1;
                    }
                    else
                    {
                        splatmapData[x, y, 0] = 0;
                        splatmapData[x, y, 1] = 1;
                        splatmapData[x, y, 2] = 0;
                    }
                }
            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }

    public void TillTerrain(Vector3 worldPos, int size)
    {
        Vector3 offsetPos = worldPos - new Vector3(size / 2, 0, size / 2);
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, size, size);
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                foreach (int layer in unfarmableLayers)
                {
                    if (splatmapData[x, y, layer] > 0.5F) return;
                }
                if (cycle.weather == Weather.Rainy)
                {
                    splatmapData[x, y, 0] = 0;
                    splatmapData[x, y, 1] = 0;
                    splatmapData[x, y, 2] = 1;
                }
                else
                {
                    splatmapData[x, y, 0] = 0;
                    splatmapData[x, y, 1] = 1;
                    splatmapData[x, y, 2] = 0;
                }
            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }
    public void TillTerrain(Vector3 worldPos, int sizeX, int sizeY)
    {
        Vector3 offsetPos = worldPos - new Vector3(sizeX / 2, 0, sizeY / 2);
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, sizeX, sizeY);

        for (var y = 0; y < sizeY; y++)
        {
            for (var x = 0; x < sizeX; x++)
            {
                float[] weights = new float[t.terrainData.alphamapLayers];

                foreach (int layer in unfarmableLayers)
                {
                    if (weights[layer] > 0.5F) return;
                }

                splatmapData[x, y, 0] = 0;
                splatmapData[x, y, 1] = 0;
                splatmapData[x, y, 2] = 0;
                splatmapData[x, y, 3] = 1;
                splatmapData[x, y, 4] = 0;


            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
        // t.Flush();
    }


    public void WaterTerrain(Vector3 worldPos)
    {

        Vector3 offsetPos = worldPos - offset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, defaultSize, defaultSize);


        for (var y = 0; y < defaultSize; y++)
        {
            for (var x = 0; x < defaultSize; x++)
            {
                if ((x - defaultSize / 2) * (x - defaultSize / 2) + (y - defaultSize / 2) * (y - defaultSize / 2) <= defaultSize / 2 * defaultSize / 2)
                {                     //  float val = Mathf.Clamp(0.5, 0 , splatmapData[x, y, 3])
                    float f = Mathf.Clamp(0.5F, 0, splatmapData[x, y, 1]);
                    splatmapData[x, y, 2] += f;
                    splatmapData[x, y, 1] -= f;
                }
            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }

    [Button]
    public void NormalizeTerrain()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                maps[x, y, 0] = 1;
                maps[x, y, 1] = 0;
                maps[x, y, 2] = 0;

            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    public void NormalizeTerrain(Vector3 worldPos)
    {
        Vector3 offsetPos = worldPos - offset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, defaultSize, defaultSize);


        for (var y = 0; y < defaultSize; y++)
        {
            for (var x = 0; x < defaultSize; x++)
            {
                float f = splatmapData[x, y, 3] + splatmapData[x, y, 4];

                splatmapData[x, y, 0] += f;
                splatmapData[x, y, 3] = 0;
                splatmapData[x, y, 4] = 0;
            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }

    public void WaterTerrain(Vector3 worldPos, int size)
    {

        Vector3 offsetPos = worldPos - new Vector3(size / 2, 0, size / 2);
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, size, size);

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                int j;


                float[] weights = new float[t.terrainData.alphamapLayers];

                for (int i = 0; i < splatmapData.GetLength(2); i++)
                {
                    weights[i] = splatmapData[x, y, i];
                }

                if (size % 2 == 0) j = size / 2;
                else j = (size - 1) / 2;

                int power = Mathf.Abs(x - j) + Mathf.Abs(y - j);
                float percentage = (float)(size - power) / size;


                weights[4] += percentage * weights[3];
                weights[3] -= percentage * weights[3];

                float sum = weights.Sum();
                for (int ww = 0; ww < weights.Length; ww++)
                {
                    weights[ww] /= sum;
                    splatmapData[x, y, ww] = weights[ww];
                }
            }
        }
        t.terrainData.SetAlphamaps(mapX, mapZ, splatmapData);
    }

    public bool CheckTexture(Vector3 worldPos)
    {
        if (checkOffset)
        {
            Vector3 offsetPos = worldPos - offset;
            int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
            int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);


            float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, 2, 2);
            float val = 0;
            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 2; x++)
                {
                    val += splatmapData[x, y, 1];
                    val += splatmapData[x, y, 2];
                }
            }

            print(val);
            if (val > 3)
            {
                return true;
            }
            else return false;
        }
        else
        {
            int mapX = (int)(((worldPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
            int mapZ = (int)(((worldPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);

            float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            if (splatmapData[0, 0, 1] > 0.5F || splatmapData[0, 0, 2] > 0.5F)
            {

                return true;
            }
            else return false;
        }

    }

    public float CheckWater(Vector3 worldPos)
    {
        int size = 10;
        Vector3 localOffset = new Vector3(size / 52.08F, 0, size / 52.08F);
        Vector3 offsetPos = worldPos - localOffset;
        int mapX = (int)(((offsetPos.x - t.GetPosition().x) / t.terrainData.size.x) * t.terrainData.alphamapWidth);
        int mapZ = (int)(((offsetPos.z - t.GetPosition().z) / t.terrainData.size.z) * t.terrainData.alphamapHeight);



        float[,,] splatmapData = t.terrainData.GetAlphamaps(mapX, mapZ, size, size);
        float val = 0;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                val += splatmapData[x, y, 2];
            }
        }

        return val;

    }

    [Button]
    public void RevertSoil()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                //if (Random.Range(0, 100) > revertSoilChance)
                //{
                //    continue;
                //}
                float temp = maps[x, y, 0];
                float temp2 = maps[x, y, 1];
                float sum = temp + temp2;

                maps[x, y, 0] = sum;
                maps[x, y, 1] = 0;
            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    [Button]
    public void DryTerrain()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                float temp = maps[x, y, 1];
                float temp2 = maps[x, y, 2];
                float sum = temp + temp2;

                maps[x, y, 1] = sum;
                maps[x, y, 2] = 0;
            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    [Button]
    public void WaterTerrain()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                if (maps[x, y, 2] > 0.5) { }
                float temp = maps[x, y, 1];
                float temp2 = maps[x, y, 2];
                float sum = temp + temp2;

                maps[x, y, 1] = 0;
                maps[x, y, 2] = sum;
            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    private void SaveTerrain()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        List<float> degen = new List<float>();
        List<float> degen2 = new List<float>();
        List<float> degen3 = new List<float>();

        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                degen.Add(maps[x, y, 0]);
                degen2.Add(maps[x, y, 1]);
                degen3.Add(maps[x, y, 2]);
            }
        }
        DataManager.Instance.currentSaveData.terrainMap1 = degen;
        DataManager.Instance.currentSaveData.terrainMap2 = degen2;
        DataManager.Instance.currentSaveData.terrainMap3 = degen3;
    }

    void LoadTerrain()
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                maps[x, y, 0] = DataManager.Instance.currentSaveData.terrainMap1[x + y * t.terrainData.alphamapWidth];
                maps[x, y, 1] = DataManager.Instance.currentSaveData.terrainMap2[x + y * t.terrainData.alphamapWidth];
                maps[x, y, 2] = DataManager.Instance.currentSaveData.terrainMap3[x + y * t.terrainData.alphamapWidth];
            }
        }
        t.terrainData.SetAlphamaps(0, 0, maps);
    }

}
