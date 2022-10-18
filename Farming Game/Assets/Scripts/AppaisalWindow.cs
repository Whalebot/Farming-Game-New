using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AppaisalWindow : MonoBehaviour
{
    public Skill appraisal;

    public TextMeshProUGUI title;
    public Image image;
    public GameObject quantityTab;
    public GameObject maturityTab;
    public GameObject qualityTab;
    public GameObject waterTab;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI maturityText;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void DisplayUI(PlantScript plant)
    {
        if (appraisal.level <= 0) return;

        if (appraisal.level > 0)
        {
            title.text = plant.SO.title + " Plant";
            image.sprite = plant.SO.sprite;

        }
        if (appraisal.level > 1)
            quantityText.text = "" + plant.quantity;
        quantityTab.SetActive(appraisal.level > 1);

        if (appraisal.level > 2)
        {
            qualityText.text = "" + plant.quality;
        }
        qualityTab.SetActive(appraisal.level > 2);

        if (appraisal.level > 3)
        {
            waterText.text = "" + plant.CheckPlantWater() + "%";
        }
        waterTab.SetActive(appraisal.level > 3);

        if (appraisal.level > 4)
        {
            maturityText.text = 100 * plant.phase / plant.phaseObjects.Length + "%";
        }
        maturityTab.SetActive(appraisal.level > 4);
    }
}
