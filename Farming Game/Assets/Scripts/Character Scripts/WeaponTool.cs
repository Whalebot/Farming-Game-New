using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : Weapon
{
    ToolScript toolScript;
    public Element element;
    public float opacity = 0.1F;
    public override void Awake()
    {
        base.Awake();
        toolScript = GetComponentInParent<ToolScript>();
    }

    public override void FixedUpdateElement()
    {
        base.FixedUpdateElement();
        if (element == Element.Water) {
            toolScript.WaterSoil(opacity);
        }
    }

    public override void HitboxStartElement()
    {
        base.HitboxStartElement();
        if (element == Element.Earth)
        {
            toolScript.TillSoil();
        }
    }
}
