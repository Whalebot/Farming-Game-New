using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterAnimator : MonoBehaviour
{
    private Status status;
    private Animator anim;
    private Movement movement;
    private AttackScript attack;
    public int frame;
    public bool hitstop;
    private float runSpeed;
    private Character character;
    float x, y;
    float zeroFloat = 0f;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    private float deaccelerateSpeed;
    float tempDirection = 0F;
    public float cycleOffset;
    public bool cycleSwitch;
    public float savedOffset;
    public float testOffset;
    //AI ai;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponentInParent<Status>();
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        //dodge = GetComponentInParent<Dodge>();
        attack = GetComponentInParent<AttackScript>();
        //  ai = GetComponentInParent<AI>();

        character = status.character;

        status.hitstunEvent += HitStun;

        if (movement != null)
        {
            movement.jumpEvent += Jump;
            movement.doubleJumpEvent += DoubleJump;
        }

        //if (dodge != null)
        //    dodge.dashStartEvent += DodgeAnimation;

        GameManager.Instance.advanceGameState += ExecuteFrame;

        if (attack != null)
        {
            attack.startupEvent += StartAttack;
            attack.recoveryEvent += AttackRecovery;
            status.parryEvent += ParryAnimation;
            status.blockEvent += Block;
        }

        //if (ai != null)
        //{
        //    ai.detectEvent += DetectAnimation;
        //}
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
        if (attack != null)
        {
            attack.startupEvent -= StartAttack;
            attack.recoveryEvent -= AttackRecovery;
            status.parryEvent -= ParryAnimation;
            status.blockEvent -= Block;
        }
        if (movement != null)
        {
            movement.jumpEvent -= Jump;
            movement.doubleJumpEvent -= DoubleJump;
        }
    }

    void DetectAnimation()
    {
        anim.SetBool("Detect", true);
    }

    void ExecuteFrame()
    {
        anim.enabled = true;
        frame = Mathf.RoundToInt(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f));

        StatusAnimation();
        BlockAnimation();
        MovementAnimation();
        if (hitstop)
            anim.enabled = false;

        if (!GameManager.Instance.runNormally) StartCoroutine(PauseAnimation());
    }

    //public void HitStop()
    //{
    //    StartCoroutine(HitstopStart());
    //}

    //IEnumerator HitstopStart()
    //{
    //    hitstop = false;
    //    yield return new WaitForFixedUpdate();
    //    // yield return new WaitForFixedUpdate();
    //    hitstop = true;
    //}

    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        Debug.Log("DisableAnim");
        anim.enabled = false;
    }

    void BlockAnimation()
    {
        // anim.SetBool("Blocking", attack.block);
    }

    void Block()
    {
        anim.SetTrigger("Block");
    }

    public void ParryAnimation()
    {
        anim.SetTrigger("Parry");
    }

    public void RefillCan()
    {
        anim.SetInteger("AttackID", 32);
        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
    }


    public void EatAnimation()
    {
        anim.SetInteger("AttackID", 100);
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
    }

    public void EquipAnimation()
    {
        anim.SetInteger("AttackID", 102);
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
    }

    public void PlantAnimation()
    {

        anim.SetInteger("AttackID", 103);
        anim.SetTrigger("Attack");
        // anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
    }


    public void UnequipAnimation()
    {
        anim.SetInteger("AttackID", 101);
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
    }

    void StatusAnimation()
    {
        anim.SetBool("Dead", status.isDead);
        anim.SetBool("Hitstun", status.inHitStun);
        anim.SetBool("InAnimation", status.currentState == Status.State.InAnimation || status.currentState == Status.State.Blockstun);

        anim.SetFloat("AttackSpeed", status.currentStats.attackSpeed);
        anim.SetFloat("MovementSpeed", status.currentStats.movementSpeedModifier);
    }

    void HitStun()
    {
        anim.SetFloat("HitX", status.knockbackDirection.x);
        anim.SetFloat("HitY", status.knockbackDirection.y);
        anim.SetTrigger("Hit");

    }

    void MovementAnimation()
    {
        if (movement == null) return;
        RunSpeed();
        tempDirection = Mathf.Sign(movement.deltaAngle);
        // anim.SetFloat("Direction", tempDirection);

        anim.SetBool("Walking", movement.isMoving);

        anim.SetBool("Strafe", movement.strafe && !movement.sprinting);
        x = Mathf.Lerp(x, movement.RelativeToForward().normalized.x, maxSpeed);
        y = Mathf.Lerp(y, movement.RelativeToForward().normalized.z, maxSpeed);

        anim.SetBool("Ground", movement.ground);

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);

        if (movement.rb.velocity.y < -0.5F)
            anim.SetInteger("Falling", -1);
        else anim.SetInteger("Falling", 1);

        anim.SetFloat("CycleOffset", cycleOffset);

        if (movement.isMoving)
        {
            cycleOffset = savedOffset;
            cycleSwitch = true;
        }
        if (cycleSwitch && !movement.isMoving)
        {
            savedOffset = testOffset;
            cycleSwitch = false;
        }

        if (movement.isMoving)
            testOffset = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    void Release() { anim.SetTrigger("Release"); }

    void StartAttack()
    {

        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        anim.SetInteger("AttackID", attack.attackID);

    }
    void AttackRecovery()
    {
        anim.SetBool("Attacking", false);
    }



    private void RunSpeed()
    {
        //runSpeed = movement.actualVelocity / movement.sprintSpeed;
        if (!movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0, deaccelerateSpeed);
        else if (movement.sprinting) runSpeed = Mathf.Lerp(runSpeed, 1, deaccelerateSpeed);
        else if (movement.run) runSpeed = Mathf.Lerp(runSpeed, 0.6F, deaccelerateSpeed);
        else if (movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0.25F, deaccelerateSpeed);


        anim.SetFloat("RunSpeed", Mathf.Abs(runSpeed));
    }

    void Jump()
    {
        anim.SetTrigger("Jump");
    }

    void DoubleJump()
    {
        anim.SetTrigger("DoubleJump");
    }

    void Land()
    {
    }
    void Hit()
    {

    }
}
