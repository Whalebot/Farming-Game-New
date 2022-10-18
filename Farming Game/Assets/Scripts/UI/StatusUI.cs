using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI strengthText;
    public Status status;
    // Start is called before the first frame update
    void Start()
    {
     // status =  GetComponentInParent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        nameText.text = status.character.characterName;
        healthText.text = status.Health + "/" + status.baseStats.maxHealth;
        staminaText.text = status.Fatigue + "/" + status.baseStats.maxStamina;
        strengthText.text = " " + (int)(status.baseStats.damageModifierPercentage * 10F);
        // statText.text = "Health: " + status.Health + "/" + status.baseStats.maxHealth;
    }
}
