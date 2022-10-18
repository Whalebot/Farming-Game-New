using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalendarScript : MonoBehaviour
{
    public TextMeshProUGUI seasonName;
    public DescriptionWindow descriptionWindow;
    public List<TimeEvent> timeEvents;
    public CalendarSlot[] calendarSlots;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnEnable()
    {
        //  UpdateCalendar();
        UIManager.Instance.SetActive(calendarSlots[0].gameObject);
    }

    public void UpdateCalendar() {
        seasonName.text = "" + TimeManager.Instance.season;

        foreach (TimeEvent timeEvent in timeEvents)
        {
            if (TimeManager.Instance.season == timeEvent.season) {
                calendarSlots[timeEvent.date - 1].timeEvent = timeEvent;
            }
        }
    }

    public void UpdateDescription(TimeEvent timeEvent, int date) {
        if (timeEvent == null)
        {
            descriptionWindow.descriptionText.text = "";
            descriptionWindow.quantityText.text = "";
        }
        else
        {
            descriptionWindow.quantityText.text = timeEvent.eventName;
            descriptionWindow.descriptionText.text = timeEvent.eventDescription;
        }
       
        descriptionWindow.titleText.text = "" + TimeManager.Instance.season + " " + date;

    }
}
