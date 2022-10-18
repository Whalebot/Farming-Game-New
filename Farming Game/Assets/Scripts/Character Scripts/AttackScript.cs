using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AttackScript : MonoBehaviour
{
    private Status status;
    [FoldoutGroup("Components")] public Transform hitboxContainer;
    [FoldoutGroup("Components")] public List<GameObject> hitboxes;

    Movement movement;
    CharacterSFX sfx;

    public delegate void AttackEvent();
    public AttackEvent startupEvent;
    public AttackEvent activeEvent;
    public AttackEvent recoveryEvent;
    public AttackEvent parryEvent;
    public AttackEvent blockEvent;
    public AttackEvent jumpEvent;
    public AttackEvent jumpCancelEvent;
    public delegate void MoveEvent(Move move);
    public MoveEvent activateHitboxEvent;
    public MoveEvent deactivateHitboxEvent;
    public MoveEvent attackHitEvent;
    public MoveEvent attackPerformedEvent;

    public Moveset moveset;
    [HeaderAttribute("Attack attributes")]
    [FoldoutGroup("Debug")] public Move activeMove;
    [FoldoutGroup("Debug")] public bool hit;
    [FoldoutGroup("Debug")] public bool gatling;
    [FoldoutGroup("Debug")] public int gatlingFrame;
    [FoldoutGroup("Debug")] public int attackID;
    [FoldoutGroup("Debug")] public int attackFrames;
    [FoldoutGroup("Debug")] public int extendedBuffer;

    [FoldoutGroup("Debug")] public int movementFrames;
    [FoldoutGroup("Debug")] public List<GameObject> projectiles;
    [FoldoutGroup("Debug")] public bool inMomentum;
    [FoldoutGroup("Jump Startup")] public int jumpFrameCounter;
    [FoldoutGroup("Jump Startup")] public int jumpActionDelay;
    [FoldoutGroup("Jump Startup")] public int jumpActionDelayCounter;
    [FoldoutGroup("Jump Startup")] public bool jumpDelay;
    [FoldoutGroup("Move properties")] public bool attacking;
    [FoldoutGroup("Move properties")] public bool attackString;
    [FoldoutGroup("Move properties")] public bool canTargetCombo;
    [FoldoutGroup("Move properties")] public bool landCancel;
    [FoldoutGroup("Move properties")] public bool jumpCancel;
    [FoldoutGroup("Move properties")] public bool specialCancel;
    [FoldoutGroup("Move properties")] public bool recoverOnlyOnLand;
    [HideInInspector] public bool newAttack;
    [HideInInspector] public bool landCancelFrame;
    [HideInInspector] public int combo;
    public int superCounter;
    List<Move> usedMoves;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<Status>();
        movement = GetComponent<Movement>();
        sfx = GetComponentInChildren<CharacterSFX>();
        movement.jumpEvent += JumpCancel;
        movement.doubleJumpEvent += JumpCancel;
        movement.landEvent += Land;
        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += HitstunEvent;

        GameManager.Instance.advanceGameState += ExecuteFrame;
        GameManager.Instance.resetEvent += ResetAttack;
    }

    private void OnDisable()
    {
        movement.jumpEvent -= JumpCancel;
        movement.doubleJumpEvent -= JumpCancel;
        movement.landEvent -= Land;
        status.neutralEvent -= ResetCombo;
        status.hurtEvent -= HitstunEvent;
        status.deathEvent -= HitstunEvent;

        GameManager.Instance.advanceGameState -= ExecuteFrame;
        GameManager.Instance.resetEvent -= ResetAttack;
    }

    private void Awake()
    {
        usedMoves = new List<Move>();
    }

    void ResetAttack()
    {
        ResetAllValues();
    }

    public void GatlingStart(bool g)
    {
        canTargetCombo = true;
        gatling = g;
        gatlingFrame = attackFrames;
    }

    public void ExecuteFrame()
    {
        if (status.hitstopCounter <= 0)
        {
            landCancelFrame = false;
            if (jumpFrameCounter > 0)
            {
                jumpFrameCounter--;
                if (jumpFrameCounter <= 0)
                {
                    status.GoToState(Status.CombatState.Neutral);
                    jumpActionDelayCounter = jumpActionDelay;
                }
            }
            if (jumpActionDelayCounter > 0)
            {
                jumpDelay = true;
                jumpActionDelayCounter--;
                if (jumpActionDelayCounter <= 0)
                {
                    jumpDelay = false;
                }
            }

            if (attacking)
            {
                attackFrames++;
                if (attackFrames > gatlingFrame + activeMove.attacks[0].gatlingFrames)
                {
                    attackString = true;
                    newAttack = false;
                }
                if (attackFrames > activeMove.firstStartupFrame + activeMove.attacks[0].gatlingFrames)
                {
                    canTargetCombo = true;
                }
                if (extendedBuffer > 0)
                    extendedBuffer--;

                //Execute properties
                ProcessInvul();
                ApplyScreenShake();
                SpawnFX();

                //Execute momentum
                bool tempMomentum = false;

                for (int i = 0; i < activeMove.m.Length; i++)
                {
                    if (attackFrames > activeMove.m[i].startFrame && attackFrames < activeMove.m[i].startFrame + activeMove.m[i].duration) { tempMomentum = true; }


                    if (attackFrames > activeMove.m[i].startFrame + activeMove.m[i].duration || status.combatState == Status.CombatState.Recovery)
                    {
                        if (activeMove.m[i].resetVelocityDuringRecovery)
                        {
                            status.rb.velocity = Vector3.zero;
                        }
                    }
                    else if (attackFrames > activeMove.m[i].startFrame)
                    {
                        Vector3 targetNoY = movement.strafeTarget.position;
                        targetNoY.y = transform.position.y;
                        float angle = Vector3.SignedAngle(transform.forward, (targetNoY - transform.position).normalized, Vector3.up);
                        if (activeMove.m[i].resetVelocityDuringRecovery)
                            status.rb.velocity = Vector3.zero;
                        if (activeMove.overrideVelocity)
                        {
                            if (activeMove.m[i].homing && Mathf.Abs(angle) < 90)
                                status.rb.velocity = (transform.right * activeMove.m[i].momentum.x) + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z;
                            else status.rb.velocity = activeMove.m[i].momentum.x * transform.right + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z;
                        }
                    }
                }

                inMomentum = tempMomentum;

                int firstStartupFrame = activeMove.attacks[0].startupFrame;
                int lastActiveFrame = activeMove.attacks[activeMove.attacks.Length - 1].startupFrame + activeMove.attacks[activeMove.attacks.Length - 1].activeFrames - 1;
                int totalMoveDuration = lastActiveFrame + activeMove.recoveryFrames;

                if (attackFrames > totalMoveDuration)
                {
                    Idle();
                }
                else if (attackFrames < firstStartupFrame)
                {
                    StartupFrames();
                }
                else if (attackFrames <= lastActiveFrame)
                {
                    ActiveFrames();
                    if (recoverOnlyOnLand) attackFrames--;
                }

                else if (attackFrames <= totalMoveDuration)
                {
                    RecoveryFrames();
                }
            }

            if (status.combatState == Status.CombatState.Neutral || status.combatState == Status.CombatState.Blockstun || status.combatState == Status.CombatState.Hitstun) usedMoves.Clear();
        }
    }

    public void SpawnFX()
    {
        if (activeMove != null)
        {
            if (activeMove.vfx.Length > 0)
                foreach (var item in activeMove.vfx)
                {
                    if (attackFrames == item.startup)
                    {
                        GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, hitboxContainer);
                        fx.transform.localPosition = item.position;
                        fx.transform.localRotation = Quaternion.Euler(item.rotation);
                        fx.transform.localScale = item.scale;
                        fx.transform.SetParent(null);
                    }
                }
            if (activeMove.sfx.Length > 0)
                foreach (var item in activeMove.sfx)
                {
                    if (attackFrames == item.startup)
                    {
                        GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, hitboxContainer);
                        fx.transform.localPosition = item.prefab.transform.localPosition;
                        fx.transform.localRotation = item.prefab.transform.rotation;
                        fx.transform.SetParent(null);
                    }
                }
        }
    }

    void ApplyScreenShake()
    {
        for (int i = 0; i < activeMove.screenShake.Length; i++)
        {
            if (attackFrames == activeMove.screenShake[i].startup && activeMove.screenShake[i].type == ScreenShakeType.OnStartup)
                CameraManager.Instance.ShakeCamera(activeMove.screenShake[i].amplitude, activeMove.screenShake[i].duration);
        }
    }
    public void StartupFrames()
    {
        status.GoToState(Status.CombatState.Startup);
    }
    void ClearHitboxes()
    {
        deactivateHitboxEvent?.Invoke(activeMove);
        for (int i = 0; i < hitboxes.Count; i++)
        {
            if (hitboxes[i] != null)
            {
                Destroy(hitboxes[i]);
            }
        }
        hitboxes.Clear();
    }

    public void ActiveFrames()
    {
        movement.actualVelocity = 0;
        for (int i = 0; i < activeMove.attacks.Length; i++)
        {
            if (attackFrames < activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames && attackFrames >= activeMove.attacks[i].startupFrame)
            {
                status.GoToState(Status.CombatState.Active);

                if (activeMove.attacks[i].hitbox == null)
                    //Activate Weapon Default Hitbox
                    activateHitboxEvent?.Invoke(activeMove);
                //Else activate custom hitbox
                else if (activeMove.attacks[i].hitbox != null)
                {
                    if (hitboxes.Count < i + 1)
                    {

                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                            hitboxes[i].transform.SetParent(null);
                        }
                        else
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                        }
                        Hitbox hitbox = hitboxes[i].GetComponentInChildren<Hitbox>();
                        hitbox.hitboxID = i;
                        hitbox.attack = this;
                        hitbox.status = status;
                        hitbox.move = activeMove;
                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            projectiles.Add(hitboxes[i]);
                            hitboxes[i] = null;
                        }
                    }
                }
            }
            else if (attackFrames > activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames)
            {
                if (activeMove.attacks[i].hitbox == null)
                    deactivateHitboxEvent?.Invoke(activeMove);
                else if (activeMove.attacks[i].hitbox != null)
                {
                    if (hitboxes.Count == i + 1)
                    {
                        Destroy(hitboxes[i]);
                    }
                }
            }
        }
    }

    public void RecoveryFrames()
    {

        newAttack = false;
        status.GoToState(Status.CombatState.Recovery);
        ClearHitboxes();
    }

    void ProcessInvul()
    {
        //Execute properties
        //Invul
        if (activeMove.invincible)
        {
            if (attackFrames == activeMove.invincibleStart)
            {
                status.invincible = true;
            }
            else if (attackFrames >= activeMove.invincibleStart + activeMove.invincibleDuration)
            {
                status.invincible = false;
            }
        }
        //Noclip
        if (activeMove.noClip)
        {
            if (attackFrames == activeMove.noClipStart)
                status.DisableCollider();
            else if (attackFrames >= activeMove.noClipStart + activeMove.noClipDuration)
            {
                status.EnableCollider();
            }
        }
        //Projectile Invul
        if (activeMove.projectileInvul)
        {
            if (attackFrames == activeMove.projectileInvulStart)
                status.projectileInvul = true;
            else if (attackFrames >= activeMove.projectileInvulStart + activeMove.projectileInvulDuration)
            {
                status.projectileInvul = false;
            }
        }
        //air Invul
        if (activeMove.airInvul)
        {
            if (attackFrames == activeMove.airInvulStart)
                status.airInvul = true;
            else if (attackFrames >= activeMove.airInvulStart + activeMove.airInvulDuration)
            {
                status.airInvul = false;
            }
        }
    }

    public void AttackProperties(Move move)
    {
        if (move == null)
        {
            print(move);
            return;
        }
        usedMoves.Add(move);
        ClearHitboxes();

        if (move.resetGatling) usedMoves.Clear();

        if (move.type == MoveType.Movement)
        {
            movementFrames = GameManager.Instance.gameFrameCount;
        }

        if (move.instantStartupRotation) movement.AttackRotation();

        recoverOnlyOnLand = move.recoverOnlyOnLand;
        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        canTargetCombo = false;
        hit = false;
        gatling = false;
        jumpCancel = false;
        specialCancel = false;

        attackFrames = 0;



        //Run momentum
        if (move.overrideVelocity) status.rb.velocity = Vector3.zero;
        else if (move.runMomentum) status.rb.velocity = status.rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) movement.performedJumps++;

        Startup();
        landCancel = move.landCancel;

        attackPerformedEvent?.Invoke(move);
        startupEvent?.Invoke();
        attacking = true;
        newAttack = true;
        //movement.isMoving = false;
        ExecuteFrame();
    }

    public bool CanUseMove(Move move)
    {
        if (move == null) return false;


        if (jumpFrameCounter > 0) return false;
        if (move.useAirAction && !attacking)
        {
            //if (movement.performedJumps <= 0)
            //{
            //    movement.performedJumps++;
            //    return true;
            //}
            //else return false;
        }

        if (!attacking) return true;

        if (activeMove != null)
        {

            if (activeMove.uncancelable) return false;
            else if (move.fullCancel) return true;
        }


        if (specialCancel)
        {
            if (move.type == MoveType.Special || move.type == MoveType.EX || move.type == MoveType.Super)
                return true;
        }

        if (move != null && canTargetCombo)
        {
            if (move.gatlingCancel) return true;
        }

        if (attacking && gatling)
        {
   
            if (activeMove.gatlingMoves.Count <= 0) return false;
            if (move == null) return true;
            if (!activeMove.gatlingMoves.Contains(move)) return false;
            else
            {
                return true;
            }
        }
        return false;
    }

    public bool TargetCombo(Move move)
    {
        //if (jumpFrameCounter > 0) return false;
        if (move == null) return false;
        if (move.useAirAction)
        {
            if (movement.performedJumps > movement.multiJumps)
            {
                return false;
            }
        }
        if (attacking && canTargetCombo)
        {
            if (activeMove.targetComboMoves.Count > 0)
            {
                if (activeMove.targetComboMoves.Contains(move))
                {
                    AttackProperties(move);
                    return true;
                }

                if (usedMoves.Contains(move) && activeMove == move || move.targetComboMoves.Contains(activeMove))
                {
                    Attack(move.targetComboMoves[0]);
                    return true;
                }

            }
        }
        return false;

    }

    public bool HasBeenUsed(Move move)
    {
        if (usedMoves.Contains(move))
        {
            int duplicates = 1;
            foreach (var item in move.gatlingMoves)
            {
                if (item == move) duplicates++;
            }
            foreach (var item in usedMoves)
            {
                if (item == move) duplicates--;
            }
            return duplicates <= 0;
        }
        else return false;
    }
    public bool Attack(Move move)
    {
        //if (jumpDelay) return false;

        if (TargetCombo(move))
        {
            return true;
        }
        if (HasBeenUsed(move))
        {
            return false;
        }
        if (!CanUseMove(move))
        {
            return false;
        }
        else
        {
            AttackProperties(move);
            return true;
        }

    }

    void Startup()
    {
        status.GoToState(Status.CombatState.Startup);
    }


    void Land()
    {
        if (recoverOnlyOnLand)
        {
            attackFrames = activeMove.lastActiveFrame + 1;
        }
        recoverOnlyOnLand = false;
        if (activeMove != null)
            for (int i = 0; i < activeMove.screenShake.Length; i++)
            {
                if (activeMove.screenShake[i].type == ScreenShakeType.OnLand)
                    CameraManager.Instance.ShakeCamera(activeMove.screenShake[i].amplitude, activeMove.screenShake[i].duration);
            }

        if (landCancel)
        {
            Debug.Log("Land Cancel");
            newAttack = false;
            landCancelFrame = true;
            Idle();
        }
    }

    void ResetCombo()
    {
        combo = 0;
    }

    void HitstunEvent()
    {
        ResetAllValues();
    }

    public void JumpCancel()
    {
        if (attacking)
        {
            status.rb.velocity = Vector3.zero;
            jumpCancelEvent?.Invoke();
            status.GoToState(Status.CombatState.Recovery);
            Idle();
        }
        attackString = false;
        canTargetCombo = false;

        if (activeMove != null)
        {
            activeMove = null;
        }

        combo = 0;
        ClearHitboxes();
        attacking = false;
        landCancel = false;
        recoveryEvent?.Invoke();
    }

    public void ResetAllValues()
    {
        ClearHitboxes();
        newAttack = false;
        attackString = false;
        if (activeMove != null)
        {
            activeMove = null;
        }

        extendedBuffer = 0;
        combo = 0;
        recoverOnlyOnLand = false;
        jumpFrameCounter = 0;
        specialCancel = false;
        attacking = false;
        gatling = false;
        canTargetCombo = false;
        landCancel = false;
        hit = false;
        status.counterhitState = false;
        status.projectileInvul = false;
        status.invincible = false;


        recoveryEvent?.Invoke();
        usedMoves.Clear();
    }

    public void Idle()
    {
        ResetAllValues();
        status.GoToState(Status.CombatState.Neutral);
    }
}
