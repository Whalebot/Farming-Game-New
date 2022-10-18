using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarSlot : MonoBehaviour
{
    Button thisButton;
    CalendarScript calendar;
    public TimeEvent timeEvent;
    public Image itemSprite;
    public GameObject hoverSFX;
    public GameObject clickSFX;
    public bool selected = false;
    public int date;
    public GameObject highlight;

    // Start is called before the first frame update
    void Awake()
    {
        calendar = GetComponentInParent<CalendarScript>();
    }

    public void OnHover()
    {
        selected = true;
        //Instantiate(hoverSFX);

        calendar.UpdateDescription(timeEvent, date);
    }
    public void Deselect()
    {
        selected = false;
    }

    void WestButton()
    {
    }

    private void OnValidate()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        itemSprite.enabled = timeEvent != null;
        if (timeEvent != null)
            itemSprite.sprite = timeEvent.icon;
        if (TimeManager.Instance != null)
        {
            highlight.SetActive(TimeManager.Instance.date == date);
        }
    }

    private void OnEnable()
    {
        UpdateSprite();
    }

    void ButtonClicked()
    {
        //  if (InputManager.buttonUp)

        Instantiate(clickSFX);

    }
}
