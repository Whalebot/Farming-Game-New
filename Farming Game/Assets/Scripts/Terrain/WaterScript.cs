using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public Season season;
    public Renderer rend;
    public Collider col;
    public Material waterMat;
    public Material iceMat;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        UpdateMat();
    }

    public void UpdateMat()
    {
        if (season == Season.Winter)
        {
            //col.enabled = true;
            rend.material = iceMat;
        }
        else {
            rend.material = waterMat;
        }
    }
}
