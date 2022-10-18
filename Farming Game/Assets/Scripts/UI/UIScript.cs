using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public Status status;
    public Slider hpBar;
    public Slider hpBar2;
    public Slider staminaBar;
    public Slider staminaBar2;
    public Text levelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;

    RectTransform healthTransform;
    RectTransform staminaTransform;
    float startStaminaWidth;
    public float delay;
    int tempHP;
    float tempStamina;
    private void Awake()
    {
        status = transform.parent.GetComponentInChildren<Status>();
        healthTransform = hpBar.GetComponent<RectTransform>();
        staminaTransform = staminaBar.GetComponent<RectTransform>();
        startStaminaWidth = staminaTransform.sizeDelta.x;


    }

    // Start is called before the first frame update
    void Start()
    {
        hpBar2.value = status.currentStats.currentHealth * 100;
        staminaBar2.value = status.currentStats.maxStamina * 100;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        hpBar.maxValue = status.currentStats.maxHealth * 100;
        staminaBar.maxValue = status.currentStats.maxStamina * 100;

        hpBar2.maxValue = status.currentStats.maxHealth * 100;
        staminaBar2.maxValue = status.baseStats.maxStamina * 100;

        //levelText.text = status.level.ToString();

        //healthTransform.sizeDelta = new Vector2(startStaminaWidth * status.maxHealth/status.startHealth, staminaTransform.sizeDelta.y);

        UpdateUI();
    }

    void UpdateUI()
    {

        staminaBar.value = (int)status.currentStats.currentStamina * 100;
        staminaText.text = (int)status.currentStats.currentStamina + "/" + (int)status.currentStats.maxStamina;
        if (status.currentStats.maxStamina != tempStamina)
        {
            staminaTransform.sizeDelta = new Vector2(startStaminaWidth * (status.currentStats.maxStamina) / status.baseStats.maxStamina, staminaTransform.sizeDelta.y);
            StopCoroutine("DelayStamina");
            StartCoroutine("DelayStamina");
        }
        tempStamina = status.currentStats.maxStamina;
        if (status.currentStats.currentHealth != tempHP)
        {
            tempHP = status.currentStats.currentHealth;
            hpBar.value = status.currentStats.currentHealth * 100;
            healthText.text = status.Health + "/" + status.currentStats.maxHealth;
            StopCoroutine("DelayHP");
            StartCoroutine("DelayHP");
        }
    }

    IEnumerator DelayHP()
    {
        yield return new WaitForSeconds(delay);
        hpBar2.value = status.currentStats.currentHealth * 100;
    }

    IEnumerator DelayStamina()
    {
        yield return new WaitForSeconds(delay);
        staminaBar2.value = status.currentStats.maxStamina * 100;
    }
}
