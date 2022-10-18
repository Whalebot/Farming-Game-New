using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantData
{
    public int plantID;
    public int phase;
    public int quality;
    public float waterLevel;
    public float sunLevel;
    public Vector3 position;
    public Quaternion rotation;
}