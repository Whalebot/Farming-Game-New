using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ItemScript
{
    AttackScript attack;
    ParticleSystem PS;
    public Hitbox defaultHitbox;
    bool isActive;
    public override void Awake()
    {
        attack = GetComponentInParent<AttackScript>();
        if (attack == null)
        {
            SetupComponent();
            return;
        }


        PS = GetComponentInChildren<ParticleSystem>();
        attack.activateHitboxEvent += EnableHitbox;
        attack.deactivateHitboxEvent += DisableHitbox;

        if (PS != null)
            PS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        defaultHitbox.attack = attack;
    }

    private void OnDisable()
    {
        if (attack == null)
        {
            ResetComponent();
            return;
        }
        attack.activateHitboxEvent -= EnableHitbox;
        attack.deactivateHitboxEvent -= DisableHitbox;
    }

    public virtual void HitboxStartElement()
    {

    }

    public virtual void FixedUpdateElement() { 
    
    }

    public void EnableHitbox(Move move)
    {
        FixedUpdateElement();
        if (isActive) return;
        HitboxStartElement();
        isActive = true;
        // Debug.Log("Enable " + move + attack.attackFrames);
        defaultHitbox.move = move;
        defaultHitbox.equipment = (EquipmentSO)SO;
        defaultHitbox.gameObject.SetActive(true);

        if (PS != null)
            PS.Play();
    }
    public void DisableHitbox(Move move)
    {
        if (!isActive)
        {

            return;
        }
        isActive = false;
        // Debug.Log("Disable " + move + attack.attackFrames);
        defaultHitbox.ResetHitbox();
        defaultHitbox.move = move;
        defaultHitbox.gameObject.SetActive(false);

        if (PS != null)
            PS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
