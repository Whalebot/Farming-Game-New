using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
public class DevCheats : MonoBehaviour
{
    public Item[] items;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button("Give Item")]
    public void GiveItem() {
        foreach (Item item in items)
        {
            InventoryScript.Instance.PickupItem(item);
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame) GiveItem();
        if (Keyboard.current.f2Key.wasPressedThisFrame) TimeManager.Instance.DayPass();
        if (Keyboard.current.f3Key.wasPressedThisFrame) TerrainScript.Instance.RevertSoil();
        if (Keyboard.current.digit5Key.wasPressedThisFrame) TimeManager.Instance.clockTime.x = 22;
    }
}
