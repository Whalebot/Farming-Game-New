using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatUpdateUI : MonoBehaviour
{
    bool isOpen;
    public GameObject container;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI healthNew;
    public TextMeshProUGUI healthOld;
    public TextMeshProUGUI staminaNew;
    public TextMeshProUGUI staminaOld;
    public TextMeshProUGUI strengthNew;
    public TextMeshProUGUI strengthOld;

    public TextMeshProUGUI skillTitle;
    public Image skillImage;
    public Status status;
    // Start is called before the first frame update
    void Awake()
    {
        status = GetComponentInParent<Status>();
    }

    private void Start()
    {
        InputManager.Instance.southInput += CloseUI;
        InputManager.Instance.westInput += CloseUI;
    }

    public void SetUI(ItemSO item, Stats oldStats, Skill skill)
    {
        isOpen = true;
        container.SetActive(true);
        UIManager.prioritizeUI = true;
        itemTitle.text = item.title;
        //if(healthNew)
        healthNew.text = "" + status.baseStats.maxHealth;
        healthOld.text = "" + oldStats.maxHealth;
        staminaNew.text = "" + status.baseStats.maxStamina;
        staminaOld.text = "" + oldStats.maxHealth;
        strengthNew.text = "" + (int)(status.baseStats.damageModifierPercentage * 10);
        strengthOld.text = "" + (int)(oldStats.damageModifierPercentage * 10);

        if (skill != null)
        {
            skillTitle.text = skill.title;
            skillImage.sprite = skill.sprite;
        }
    }

    void CloseUI()
    {
        if (!isOpen) return;
        isOpen = false;
        UIManager.prioritizeUI = false;
        container.SetActive(false);
    }
}
