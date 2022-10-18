using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DescriptionWindow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI levelText;
    public GameObject equipWindow;
    public GameObject freshnessWindow;

    public TextMeshProUGUI attackText;
    public Slider expSlider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    string CheckStringTags(Skill skill)
    {
        string s = skill.description;

        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            //foreach (string tag in blueTag.tags)
            {
                if (words[i].ToLower().Contains("value"))
                {
                    words[i] = "<color=#00ffffff>" + words[i] + "</color>";
                }
            }
        }
        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }




    public void DisplayUI(ItemSO item)
    {

        if (icon != null)
            icon.sprite = item.sprite;
        if (titleText != null)
            titleText.text = item.title;
        if (descriptionText != null)
            descriptionText.text = item.description;
        if (quantityText != null)
            quantityText.text = "" + InventoryScript.Instance.CheckQuantity(item);
        if (priceText != null)
            priceText.text = "" + item.baseValue;




        if (equipWindow != null)
        {
      
            if (item.GetType() == typeof(EquipmentSO))
            {
                equipWindow.SetActive(true);
                EquipmentSO eq = (EquipmentSO)item;
                attackText.text = "" + eq.attack;
            }
            else
            {
                equipWindow.SetActive(false);
            }
        }


        if (statText != null)
        {
            statText.text = "";
            for (int i = 0; i < item.eventScript.statTypes.Length; i++)
            {
                statText.text += "" + item.eventScript.statTypes[i].type + ": " + item.eventScript.statTypes[i].value + "\n";
            }
        }
    }

    public void DisplayUI(Skill item)
    {
        if (icon != null)
            icon.sprite = item.sprite;
        if (titleText != null)
            titleText.text = item.title;
        if (descriptionText != null)
            descriptionText.text = CheckStringTags(item);

        expSlider.maxValue = Mathf.Pow(1.5F, item.level) * 100;
        expSlider.value = item.experience;

        levelText.text = "Lv. " + item.level;
    }

    public void DisplayUI(Item item)
    {

        if (icon != null)
            icon.sprite = item.SO.sprite;
        if (titleText != null)
            titleText.text = item.SO.title;
        if (descriptionText != null)
        {

            descriptionText.text = CheckStringTags(item);
        }

        if (quantityText != null)
            quantityText.text = "x" + item.quantity;
        if (priceText != null)
            priceText.text = "" + item.SO.baseValue;
        if (levelText != null)
            levelText.text = "" + item.quality;

        if (freshnessWindow != null)
        {
            freshnessWindow.SetActive(item.SO.ageLimit > 0);
            if (item.SO.ageLimit > 0)
            {
            
                expSlider.value = (float) (item.SO.ageLimit-item.age) / item.SO.ageLimit;
            }
        }

        if (equipWindow != null)
        {
            if (item.GetType() == typeof(EquipmentSO))
            {
                equipWindow.SetActive(true);
                EquipmentSO eq = (EquipmentSO)item.SO;
                attackText.text = "" + eq.attack;
            }
            else
            {
                equipWindow.SetActive(false);
            }
        }


        if (statText != null)
        {
            statText.text = "";
            for (int i = 0; i < item.SO.eventScript.statTypes.Length; i++)
            {
                statText.text += "" + item.SO.eventScript.statTypes[i].type + ": " + (int)(item.SO.eventScript.statTypes[i].value + item.SO.eventScript.statTypes[i].value * item.quality / 100F) + "\n";
            }
        }
    }
    string CheckStringTags(Item item)
    {
        string s = item.SO.description;
        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ' || letter == '\n')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].ToLower().Contains("stats"))
            {
                words[i] = "<color=#00ffffff>";
                for (int j = 0; j < item.SO.eventScript.statTypes.Length; j++)
                {
                    words[i] += "" + item.SO.eventScript.statTypes[j].type + " +" + (int)(item.SO.eventScript.statTypes[j].value + item.SO.eventScript.statTypes[j].value * item.quality / 100F) + "\n";
                }
                words[i] += "</color>";
            }
        }
        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }

    public void DisableDisplay()
    {

    }
}
