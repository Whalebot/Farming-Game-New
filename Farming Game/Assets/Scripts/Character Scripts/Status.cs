using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour
{
    public Character character;

    [TabGroup("Current Stats")] public Alignment alignment = Alignment.Enemy;
    [TabGroup("Current Stats")] public GroundState groundState;
    public enum State { Neutral, Hitstun, Blockstun, InAnimation, TopAnimation }
    [TabGroup("Current Stats")] public State currentState;
    public enum CombatState { Neutral, Startup, Active, Recovery, Hitstun, Blockstun, Knockdown, Wakeup, LockedAnimation }
    [TabGroup("Current Stats")] public CombatState combatState;

    [TabGroup("Current Stats")] public int hitstunValue;
    [TabGroup("Current Stats")] public int blockstunValue;
    [HideInInspector] public bool inBlockStun;
    [HideInInspector] public bool inHitStun;
    [TabGroup("Current Stats")] public int hitstopCounter;
    [TabGroup("Current Stats")] public int staminaRegenTimer = 1;
    int staminaRegenCounter;
    [TabGroup("Current Stats")]
    [HideLabel] public Stats currentStats;
    [TabGroup("Base Stats")]
    [HideLabel] public Stats baseStats;

    [TabGroup("Properties")] public bool hasArmor;
    [HideInInspector] public bool animationArmor;
    [TabGroup("Properties")] public bool counterhitState = false;
    [TabGroup("Properties")] public bool projectileInvul = false;
    [TabGroup("Properties")] public bool invincible = false;
    [TabGroup("Properties")] public bool airInvul = false;

    [Header("Auto destroy on death")]
    [TabGroup("Settings")] public bool autoDeath;
    [TabGroup("Settings")] public bool staminaDeath;
    [TabGroup("Settings")] public bool destroyParent;
    [TabGroup("Settings")] [ShowIf("autoDeath")] public float autoDeathTime = 1.5F;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector2 knockbackDirection;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool blocking;
    [HideInInspector] public bool parrying;
    [HideInInspector] public int parryStun;

    public event Action healthEvent;
    public event Action hurtEvent;
    public event Action deathEvent;
    public event Action blockEvent;
    public event Action parryEvent;

    public event Action neutralEvent;
    public event Action animationEvent;
    public event Action blockstunEvent;
    public event Action hitstunEvent;
    public event Action invincibleEvent;

    CharacterSFX characterSFX;

    [HideInInspector] public bool godMode;
    [HideInInspector] public bool regenStamina;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();
        characterSFX = GetComponentInChildren<CharacterSFX>();
        currentState = State.Neutral;

        ApplyCharacter();
    }

    private void Start()
    {
        TimeManager.Instance.sleepStartEvent += RestoreStats;
    }

    void FixedUpdate()
    {
        ResolveHitstun();
        StateMachine();
    }

    public void EnableCollider() { }
    public void DisableCollider() { }

    public void RestoreStats()
    {
        ReplaceStats(currentStats, baseStats);
    }

    public void ReplaceStats(Stats stat1, Stats stat2)
    {

        //Get stat definition and replace 1 with 2
        Stats def1 = stat1;
        Stats def2 = stat2;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
        }
    }

    void StateMachine()
    {
        switch (currentState)
        {
            case State.Neutral:
                StaminaRegen();
                break;
            case State.InAnimation: break;
            case State.Hitstun: break;
            case State.Blockstun: break;
            default: break;
        }
    }

    void StaminaRegen()
    {
        if (currentStats.currentStamina < currentStats.maxStamina)
        {
            if (regenStamina)
            {
                staminaRegenCounter--;
                if (staminaRegenCounter <= 0 && blocking)
                {
                    staminaRegenCounter = staminaRegenTimer;
                    currentStats.currentStamina += 2;
                }
                else currentStats.currentStamina += 2;
            }

            currentStats.currentStamina = Mathf.Clamp(currentStats.currentStamina, -100, currentStats.maxStamina);
        }
    }

    public void GoToState(State transitionState)
    {
        switch (transitionState)
        {
            case State.Neutral:
                if (currentStats.maxStamina <= 0 && staminaDeath) Death();
                currentState = State.Neutral;
                neutralEvent?.Invoke(); break;
            case State.InAnimation:
                currentState = State.InAnimation;
                animationEvent?.Invoke();
                break;
            case State.Hitstun:
                currentState = State.Hitstun;
                hitstunEvent?.Invoke();
                break;
            case State.Blockstun:
                currentState = State.Blockstun;
                blockstunEvent?.Invoke(); break;
            case State.TopAnimation:
                currentState = State.TopAnimation;
                break;
            default: break;
        }
    }
    public void GoToState(CombatState transitionState)
    {
        if (combatState == CombatState.Wakeup && transitionState != CombatState.Hitstun)
        {
            Instantiate(VFXManager.Instance.wakeupFX, transform.position + VFXManager.Instance.wakeupFX.transform.localPosition, Quaternion.identity);
        }

        combatState = transitionState;

        switch (transitionState)
        {
            case CombatState.Neutral:
                rb.useGravity = true;
                invincible = false;
                neutralEvent?.Invoke(); break;
            case CombatState.Startup:
                blocking = false;
                break;
            case CombatState.Active:
                blocking = false;
                break;
            case CombatState.Recovery:
                rb.useGravity = true;
                blocking = false;
                break;
            case CombatState.Hitstun:
                rb.useGravity = true;
                inHitStun = true;
                hitstunEvent?.Invoke();
                break;
            case CombatState.Blockstun:
                inBlockStun = true;
                blockstunEvent?.Invoke(); break;
            case CombatState.Knockdown:
                rb.useGravity = true;
                inHitStun = true;
                break;
            case CombatState.Wakeup:
                rb.useGravity = true;
                break;
            case CombatState.LockedAnimation:
                blocking = false;
                break;
            default: break;
        }
    }

    public void ApplyCharacter()
    {
        if (character == null) return;
        ReplaceStats(currentStats, character.stats);
        ReplaceStats(baseStats, character.stats);
    }

    public int Poise
    {
        get
        {
            return currentStats.poise;
        }
        set
        {
            if (currentStats.poise <= 0)
            {
                currentStats.poise = baseStats.poise;
            }
            else
            {
                currentStats.poise = Mathf.Clamp(value, 0, baseStats.poise);
            }
        }
    }

    public float MaxStamina
    {
        get
        {
            return baseStats.maxStamina;
        }
        set
        {
            float difference = value - baseStats.maxStamina;
            baseStats.maxStamina = value;

            currentStats.maxStamina = Mathf.Clamp(currentStats.maxStamina + difference, 0, baseStats.maxStamina);
            currentStats.currentStamina = Mathf.Clamp(currentStats.currentStamina + difference, 0, currentStats.maxStamina);
        }
    }


    public float Fatigue
    {
        get
        {
            return currentStats.maxStamina;
        }
        set
        {
            float difference = value - currentStats.maxStamina;

            currentStats.maxStamina = Mathf.Clamp(value, 0, baseStats.maxStamina);
            currentStats.currentStamina = Mathf.Clamp(currentStats.currentStamina + difference, 0, currentStats.maxStamina);

        }
    }


    public int Health
    {
        get
        {
            return currentStats.currentHealth;
        }
        set
        {
            if (isDead)
                if (currentStats.currentHealth == value) return;
            if (godMode) return;

            currentStats.currentHealth = Mathf.Clamp(value, 0, currentStats.maxHealth);

            healthEvent?.Invoke();
            if (currentStats.currentHealth <= 0 && !isDead)
            {
                Death();
            }
        }
    }

    public int HitStun
    {
        get { return hitstunValue; }
        set
        {
            if (!hasArmor && !animationArmor)
            {
                if (value <= 0) return;

                hitstunValue = value;
                GoToState(State.Hitstun);
            }
        }
    }

    public int BlockStun
    {
        get { return blockstunValue; }
        set
        {
            blockstunValue = value;
            GoToState(State.Blockstun);
        }
    }

    public void TakeHit(int damage, Vector3 kb, int stunVal, int poiseBreak, Vector3 dir, float slowDur)
    {
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        Poise -= poiseBreak;

        if (angle > 90)
        {
            if (parrying)
            {
                parryEvent?.Invoke();
                BlockStun = parryStun;
                return;
            }
            else if (blocking)
            {
                blockEvent?.Invoke();
                currentStats.currentStamina -= damage * 2;
                BlockStun = stunVal;
                if (Poise <= 0)
                {
                    TakePushback(kb);
                }
                return;
            }
        }


        if (Poise <= 0)
        {
            if (baseStats.poise < 5)
                TakePushback(kb);

            HitStun = stunVal;
            hurtEvent?.Invoke();
            GameManager.Instance.Slowmotion(slowDur);
        }
        GameManager.Instance.DamageNumbers(transform, damage);
        Health -= damage;
    }
    public void TakePushback(Vector3 direction)
    {
        float temp = Vector3.SignedAngle(new Vector3(direction.x, 0, direction.z), transform.forward, Vector3.up);
        Vector3 tempVector = (Quaternion.Euler(0, temp, 0) * new Vector3(direction.x, 0, direction.z)).normalized;
        knockbackDirection = new Vector2(tempVector.x, tempVector.z);

        if (!hasArmor && !animationArmor)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(direction, ForceMode.VelocityChange);
        }
    }

    public void Death()
    {
        isDead = true;
        deathEvent?.Invoke();
        if (autoDeath) StartCoroutine("DelayDeath");
    }

    IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(autoDeathTime);
        if (!destroyParent)
            Destroy(gameObject);
        else Destroy(transform.parent.gameObject);
    }

    void ResolveHitstun()
    {
        if (blockstunValue > 0)
        {
            inBlockStun = true;
            blockstunValue--;
        }
        else if (blockstunValue <= 0 && inBlockStun)
        {
            GoToState(State.Neutral);
            blockstunValue = 0;
            inBlockStun = false;
        }


        if (hitstunValue > 0 && !hasArmor)
        {
            hitstunValue--;
            inHitStun = true;
        }
        else if (hitstunValue <= 0 && inHitStun)
        {
            GoToState(State.Neutral);
            hitstunValue = 0;
            inHitStun = false;
        }
    }
}

public enum Alignment
{
    Player,
    Enemy
}
public enum StatusEffect { Burning, Frozen };
public enum GroundState { Grounded, Airborne, Knockdown }