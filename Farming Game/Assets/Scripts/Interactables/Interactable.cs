using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interactable : MonoBehaviour
{

    public enum Type
    {
        Talk,
        Take,
        Sleep,
        Harvest,
        Travel,
        Shop,
        Enter,
        Craft,
        Open,
        Leave,
        None
    }
    public Type type;
    // Start is called before the first frame update
    void Start()
    {

    }

    public virtual void Interact()
    {

    }

    public virtual void Interact(Item item)
    {

    }
}
