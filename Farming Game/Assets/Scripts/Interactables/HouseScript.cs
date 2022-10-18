using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : Interactable
{
    public GameObject sleepSFX;
    public bool isSleeping;


    public override void Interact()
    {
        base.Interact();
        Sleep();
    }

    public void Sleep()
    {
        if (!isSleeping)
        {
            isSleeping = true;
            Instantiate(sleepSFX);
            //   TimeManager.Instance.Sleep();
            StartPosition.spawnPointID = 1;
            GameManager.Instance.Sleep();
        }
    }
}
