using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public int animationID;
    public Sprite icon;
    public float staminaCost;
    public float fatigue;
    [Header("Read Only")]
    public int firstStartupFrame;
    public int lastActiveFrame;
    public int totalMoveDuration;
    public int firstGatlingFrame;

    [Header("Editable")]
    public int recoveryFrames;

    [TabGroup("Attacks")] public Attack[] attacks;

    [Header("Hit properties")]
    [TabGroup("Attacks")] public MoveType type;
    [TabGroup("Attacks")] [EnumToggleButtons] public DamageType damageType;
    [TabGroup("Attacks")] [Range(0, 100)] public int slashValue;
    [TabGroup("Attacks")] [Range(0, 100)] public int thrustValue;
    [TabGroup("Attacks")] [Range(0, 100)] public int bluntValue;
    [TabGroup("Attacks")] [Range(0, 100)] public int chopValue;

   [TabGroup("Group 2", "RPG")] public List<SkillExp> trainedSkills;

    [Header("Screen shake")]
    [TabGroup("Group 2", "FX")]
    public ScreenShake[] screenShake;
    [Header("Hit Stop")]
    [TabGroup("Group 2", "FX")] public float slowMotionDuration = 0.01F;
    [TabGroup("Group 2", "FX")] public bool startupParticle;
    [TabGroup("Group 2", "FX")] public VFX[] vfx;
    [TabGroup("Group 2", "FX")] public SFX[] sfx;
    [TabGroup("Group 2", "FX")] public GameObject hitFX;
    [TabGroup("Group 2", "FX")] public GameObject blockFX;
    [TabGroup("Group 2", "FX")] public GameObject counterhitFX;
    [TabGroup("Group 2", "FX")] public GameObject hitSFX;
    [TabGroup("Group 2", "FX")] public GameObject blockSFX;
    [TabGroup("Group 2", "FX")] public GameObject counterhitSFX;

    [Header("Move properties")]
    [TabGroup("Move properties")] public bool verticalRotation = true;
    [TabGroup("Move properties")] public int particleID;
    [TabGroup("Move properties")] public bool holdAttack;
    [TabGroup("Move properties")] public bool autoAim;
    [TabGroup("Move properties")] public bool tracking;
    [TabGroup("Move properties")] public bool armor;
    [TabGroup("Move properties")] public bool homing;
    [TabGroup("Move properties")] public bool resetGatling;

    [TabGroup("Cancel properties")] public List<Move> targetComboMoves;
    [TabGroup("Cancel properties")] public List<Move> gatlingMoves;
    [TabGroup("Cancel properties")] public bool fullCancel = false;
    [TabGroup("Cancel properties")] public bool gatlingCancel = false;
    [TabGroup("Cancel properties")] public bool uncancelable = false;


    [TabGroup("Cancel properties")] public bool gatlingCancelOnBlock = true;
    [TabGroup("Cancel properties")] public bool gatlingCancelOnHit = true;
    [TabGroup("Cancel properties")] public bool jumpCancelOnBlock;
    [TabGroup("Cancel properties")] public bool jumpCancelOnHit = true;
    [TabGroup("Cancel properties")] public bool specialCancelOnBlock = true;
    [TabGroup("Cancel properties")] public bool specialCancelOnHit = true;

    [TabGroup("Invul properties")] public bool noClip;
    [ShowIf("noClip")]
    [TabGroup("Invul properties")] public int noClipStart = 1;
    [ShowIf("noClip")]
    [TabGroup("Invul properties")] public int noClipDuration;
    [TabGroup("Invul properties")] public bool invincible;
    [ShowIf("invincible")]
    [TabGroup("Invul properties")] public int invincibleStart = 1;
    [ShowIf("invincible")]
    [TabGroup("Invul properties")] public int invincibleDuration;
    [TabGroup("Invul properties")] public bool projectileInvul;
    [ShowIf("projectileInvul")]
    [TabGroup("Invul properties")] public int projectileInvulStart = 1;
    [ShowIf("projectileInvul")]
    [TabGroup("Invul properties")] public int projectileInvulDuration;

    [TabGroup("Invul properties")] public bool airInvul;
    [ShowIf("airInvul")]
    [TabGroup("Invul properties")] public int airInvulStart = 1;
    [ShowIf("airInvul")]
    [TabGroup("Invul properties")] public int airInvulDuration;

    [TabGroup("Air properties")] public bool aimOnStartup;
    [TabGroup("Air properties")] public bool useAirAction;
    [TabGroup("Air properties")] public bool landCancel;
    [TabGroup("Air properties")] public bool recoverOnlyOnLand;

    [Header("Momentum")]
    [TabGroup("Momentum")] public Momentum[] m;
    [TabGroup("Momentum")] public bool instantStartupRotation = false;
    [TabGroup("Momentum")] public bool overrideVelocity = true;
    [TabGroup("Momentum")] public bool runMomentum = true;

    private void OnValidate()
    {
        if (attacks == null) return;
        if (attacks.Length <= 0) return;

        firstStartupFrame = attacks[0].startupFrame;
        firstGatlingFrame = attacks[0].startupFrame + attacks[0].gatlingFrames;
        lastActiveFrame = attacks[attacks.Length - 1].startupFrame + attacks[attacks.Length - 1].activeFrames - 1;
        totalMoveDuration = lastActiveFrame + recoveryFrames;
    }
}

public enum DamageType { Slash, Blunt, Thrust, Chop, Fire, Water, Earth, Wind }


[System.Serializable]
public class SFX
{

    public int startup = 1;
    public GameObject prefab;
}

[System.Serializable]
public class ScreenShake
{
    public ScreenShakeType type;
    [HideIf("@type != ScreenShakeType.OnStartup")] public int startup = 1;
    public float amplitude = 2;
    public int duration = 10;

}

[System.Serializable]
public class VFX
{
    public int startup = 1;
    public GameObject prefab;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
}


[System.Serializable]
public class Attack
{
    public AttackLevel attackLevel = AttackLevel.Level1;
    public AttackType attackType = AttackType.Normal;
    public GameObject hitbox;
    public int damage;
    public int stun = 20;
    public int hitstop = 5;
     public int poiseBreak;

    public int startupFrame = 10;
    public int activeFrames = 5;
    public int gatlingFrames = 5;
    public HitProperty groundHitProperty;
    public HitProperty airHitProperty;
    public HitProperty blockProperty;
    public int hitID = 0;
}

[System.Serializable]
public class Momentum
{
    public int startFrame = 1;
    public int duration;
    public Vector3 momentum;
    public bool homing = false;
    public bool resetVelocityDuringRecovery = true;

}

[System.Serializable]
public class HitProperty
{
    public Vector3 pushback;
    public HitState hitState;

}

public enum ScreenShakeType { OnStartup, OnHit, OnLand }

public enum AttackType { Normal, Projectile, Throw }
public enum AttackLevel { Level1, Level2, Level3, Custom }
public enum HitState { None, Knockdown, Launch };
public enum MoveType { Normal, Special, UniversalMechanics, Movement, EX, Super }