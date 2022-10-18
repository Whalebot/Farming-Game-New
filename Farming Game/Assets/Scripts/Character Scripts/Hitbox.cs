﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    [HideInInspector] public int totalDamage;
    [HideInInspector] public int hitboxID;
    [HideInInspector] public AttackScript attack;
    [HideInInspector] public EquipmentSO equipment;
    [HideInInspector] public Move move;
    [HideInInspector] public Status status;
    [SerializeField] protected bool canClash = true;
    Vector3 knockbackDirection;
    Vector3 aVector;
    public Transform body;
    [SerializeField] public List<Status> enemyList;
    MeshRenderer mr;
    protected bool returnWallPushback;
    protected Transform colPos;

    private void Awake()
    {
        // print(" Hitbox active");
        mr = GetComponent<MeshRenderer>();

        if (body == null) body = GetComponentInParent<Status>().transform;

        enemyList = new List<Status>();

    }
    private void OnEnable()
    {
        if (GameManager.Instance.showHitboxes)
        {
            mr.enabled = true;
        }
        else
        {
            mr.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;
        if (attack.landCancelFrame) return;
        if (hitbox != null && canClash)
        {
            if (hitbox.GetType() == typeof(Projectile)) return;
            if (CheckInvul(enemyStatus) && hitbox.CheckInvul(status))
            {
                Clash(enemyStatus);
                return;
            }
        }
        else if (enemyStatus != null)
        {
            if (status == enemyStatus) return;

            if (!enemyList.Contains(enemyStatus))
            {
                canClash = false;
                if (!CheckInvul(enemyStatus)) return;
                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }
    }

    public void ResetHitbox()
    {
        enemyList.Clear();
    }

    public bool CheckInvul(Status enemyStatus)
    {
        if (enemyStatus.invincible)
        {
            return false;
        }
        return true;
    }

    public virtual void DoDamage(Status other, float dmgMod)
    {
        Debug.Log(attack.attackFrames);
        CheckAttack(other, move.attacks[hitboxID]);
    }

    public virtual void CheckAttack(Status other, Attack tempAttack)
    {
        returnWallPushback = move.attacks[hitboxID].attackType != AttackType.Projectile;
        knockbackDirection = body.forward;
        knockbackDirection.y = 0;
        knockbackDirection = knockbackDirection.normalized;

        //Check for block
        if (other.blocking)
        {
            ExecuteBlock(tempAttack.blockProperty, other);
        }
        else
        {
            if (other.groundState == GroundState.Grounded)
            {
                ExecuteHit(tempAttack.groundHitProperty, other, tempAttack);
            }
            //Check for airborne or knockdown state
            else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
            {
                ExecuteHit(tempAttack.airHitProperty, other, tempAttack);
            }
        }
    }


    void ExecuteBlock(HitProperty hit, Status other)
    {
        if (move.gatlingMoves.Count > 0 && move.gatlingCancelOnBlock)
            attack.GatlingStart(true);
        else
            attack.GatlingStart(false);
        attack.hit = true;
        attack.specialCancel = move.specialCancelOnBlock;
        attack.jumpCancel = move.jumpCancelOnBlock;

        //Block FX
        if (move.blockFX != null)
            Instantiate(move.blockFX, colPos.position, colPos.rotation);
        else Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);
        if (move.blockSFX != null)
            Instantiate(move.blockSFX, colPos.position, colPos.rotation);
        else Instantiate(VFXManager.Instance.defaultBlockSFX, colPos.position, colPos.rotation);


        //Calculate direction
        aVector = knockbackDirection * hit.pushback.z + Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + Vector3.up * hit.pushback.y;
        //other.TakeBlock(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection, returnWallPushback);
    }



    void ExecuteHit(HitProperty hit, Status other, Attack atk)
    {
        if (move.gatlingMoves.Count > 0 && move.gatlingCancelOnHit)
            attack.GatlingStart(true);
        else
            attack.GatlingStart(false);
        attack.hit = true;
        attack.specialCancel = move.specialCancelOnHit;
        attack.jumpCancel = move.jumpCancelOnHit;

        attack.attackHitEvent?.Invoke(move);

        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        //Calculate direction
        aVector = knockbackDirection * hit.pushback.z + Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + Vector3.up * hit.pushback.y;

        //Screen shake on hit
        for (int i = 0; i < move.screenShake.Length; i++)
        {
            if (move.screenShake[i].type == ScreenShakeType.OnHit)
                CameraManager.Instance.ShakeCamera(move.screenShake[i].amplitude, move.screenShake[i].duration);
        }


        int totalDamage = (int)(atk.damage);
        if (equipment != null)
            totalDamage = (int)(atk.damage * equipment.damageMultiplier) + equipment.attack;
  

        switch (atk.attackLevel)
        {
            case AttackLevel.Level1:
                other.TakeHit(totalDamage, aVector, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, aVector, 0.1F);
                break;
            case AttackLevel.Level2:
                other.TakeHit(totalDamage, aVector, CombatManager.Instance.lvl2.stun, CombatManager.Instance.lvl2.poiseBreak, aVector, 0.1F);
                break;
            case AttackLevel.Level3:
                other.TakeHit(totalDamage, aVector, CombatManager.Instance.lvl3.stun, CombatManager.Instance.lvl3.poiseBreak, aVector, 0.1F);
                break;
            case AttackLevel.Custom:
                other.TakeHit(totalDamage, aVector, atk.stun, atk.poiseBreak, aVector, 0.1F);
                break;
        }
    }

    void Clash(Status enemyStatus)
    {
        print("Clash");
        canClash = false;
        Collider col = GetComponent<Collider>();
        col.enabled = false;
        status.hitstopCounter = 25;

        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        attack.newAttack = false;
        attack.Idle();
    }
}
