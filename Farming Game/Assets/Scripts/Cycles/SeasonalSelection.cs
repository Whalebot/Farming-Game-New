using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonalSelection : MonoBehaviour
{
    public Season season;
    public GameObject springObject;
    public GameObject summerObject;
    public GameObject autumnObject;
    public GameObject winterObject;

    // Start is called before the first frame update
    void Start()
    {


    }

    private void OnEnable()
    {
        ChangeObject();
    }

    private void OnDisable()
    {
  
    }


    private void OnValidate()
    {
        ChangeObject();
    }

    // Update is called once per frame
    void ChangeObject()
    {
        if (TimeManager.Instance != null) {
            season = TimeManager.Instance.season;
        }

        springObject.SetActive(false);
        summerObject.SetActive(false);
        autumnObject.SetActive(false);
        winterObject.SetActive(false);


        switch (season)
        {
            case Season.Spring: springObject.SetActive(true); break;
            case Season.Summer: summerObject.SetActive(true); break;
            case Season.Autumn: autumnObject.SetActive(true); break;
            case Season.Winter: winterObject.SetActive(true); break;
        }
    }
}
