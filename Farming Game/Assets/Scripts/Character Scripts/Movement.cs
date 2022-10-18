using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Movement : MonoBehaviour
{
    [HideInInspector] public Status status;
    [HideInInspector] public Rigidbody rb;



    [HeaderAttribute("Movement attributes")]
    [TabGroup("Movement"), ReadOnly] public Vector3 direction;
    [TabGroup("Movement")] public bool isMoving = false;
    [TabGroup("Movement")] public bool forwardOnly = true;
    [TabGroup("Movement")] public bool forcedWalk;
    [TabGroup("Movement")] public float walkSpeed = 3;
    [TabGroup("Movement")] public float runSpeed = 8;
    [TabGroup("Movement")] public float strafeSpeed = 5;

    [HideInInspector] public bool run;

    [TabGroup("Movement")] public float currentVel;
    [TabGroup("Movement"), ReadOnly] public float actualVelocity;
    [TabGroup("Movement")] public float smoothAcceleration = 0.5f;
    [TabGroup("Movement")] public float smoothDeacceleration = 0.5f;
    [TabGroup("Movement")] public float walkThreshold;

    [TabGroup("Rotation")]
    [HeaderAttribute("Rotation attributes")]
    [TabGroup("Rotation")] public bool smoothRotation = true;
    [TabGroup("Rotation")] public float rotationDamp = 8;
    [TabGroup("Rotation")] public float sharpRotationDamp = 16;
    [TabGroup("Rotation")] public float deltaAngle;

    [TabGroup("Strafe")] public bool strafe;
    [TabGroup("Strafe")] public Transform strafeTarget;
    [TabGroup("Strafe")] public Transform defaultStrafeTarget;

    [TabGroup("Ground Detection")] public bool ground;
    [TabGroup("Ground Detection")] public bool stairs;
    [TabGroup("Ground Detection")] public float rayLength;
    [TabGroup("Ground Detection")] public float stepRayLength;
    [TabGroup("Ground Detection")] public float stepHeight;
    [TabGroup("Ground Detection")] public float stepAngle;
    [TabGroup("Ground Detection")] public float stepSmooth;
    RaycastHit hit, hit2;
    [TabGroup("Ground Detection")] public float offset;
    [TabGroup("Ground Detection")] public LayerMask groundMask;

    [HeaderAttribute("Jump attributes")]
    [TabGroup("Jump")] public float jumpHeight;
    [TabGroup("Jump")] public float fallMultiplier;
    [TabGroup("Jump")] public int minimumJumpTime = 2;
    [TabGroup("Jump")] int jumpCounter;
    [TabGroup("Jump")] public int multiJumps;
    [TabGroup("Jump")] public int performedJumps;
    [TabGroup("Jump")] public float airRotation = 4;
    [TabGroup("Jump")] public float airDeceleration = 0.95F;
    [TabGroup("Jump")] public float airBrake = 0.8F;

    [HeaderAttribute("Sprint attributes")]
    [TabGroup("Movement")] public bool sprinting;
    [TabGroup("Movement")] public float sprintSpeed = 12;
    [TabGroup("Movement")] public float sprintRotation = 3;
    [TabGroup("Movement")] public int sprintCostTimer = 2;
    [TabGroup("Movement")] float sprintCounter;

    public event Action jumpEvent;
    public event Action doubleJumpEvent;
    public event Action landEvent;
    public event Action strafeSet;
    public event Action strafeBreak;

    [HideInInspector] public float zeroFloat;
    [FoldoutGroup("Assign components")]
    [FoldoutGroup("Assign components")] public CharacterSFX sfx;
    [FoldoutGroup("Assign components")] public Collider hurtbox;
    [FoldoutGroup("Assign components")] public Collider col;
    [FoldoutGroup("Assign components")] public PhysicMaterial groundMat;
    [FoldoutGroup("Assign components")] public PhysicMaterial airMat;
    bool check;
    bool check2;

    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponentInChildren<CharacterSFX>();
        rb = GetComponent<Rigidbody>();
        status = GetComponent<Status>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        status.deathEvent += DisableMovement;
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    void ExecuteFrame()
    {
        if (GameManager.isPaused || status.isDead)
        {
            //rb.drag = 1;
            isMoving = false;
            return;
        }
        if (status.currentState == Status.State.Neutral && status.combatState == Status.CombatState.Neutral || status.currentState == Status.State.Neutral && status.combatState == Status.CombatState.Startup || status.currentState == Status.State.TopAnimation)
        {
            MovementProperties();
            Rotation();
            PlayerMovement();
        }

        

        if (rb.velocity.y < 0) rb.velocity += Physics.gravity * fallMultiplier;
        GroundDetection();
    }

    #region Assistance functions
    public void SetStrafeTarget(Transform t)
    {
        if (t == null)
        {
            strafeTarget = defaultStrafeTarget;
        }
        else
            strafeTarget = t;

        strafe = true;
        strafeSet?.Invoke();
    }

    public void ResetStrafe()
    {
        strafeBreak?.Invoke();
        strafeTarget = defaultStrafeTarget;
        strafe = false;
    }

    void DisableMovement()
    {
        {
            rb.velocity = Vector3.zero;
            direction = Vector3.zero;
            rb.isKinematic = true;
            hurtbox.gameObject.SetActive(false);
            return;
        }
    }
    public void RotateInPlace(Vector3 dir)
    {
        deltaAngle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(dir.x, 0, dir.z), Vector3.up), 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);
    }
    #endregion
    public virtual void Rotation()
    {
        if (strafe && !sprinting && ground)
        {
            if (strafeTarget == null) return;
            Vector3 desiredDirection = strafeTarget.position - transform.position;
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            if (Mathf.Abs(deltaAngle) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * sharpRotationDamp);
            }

            return;
        }

        if (direction != Vector3.zero)
        {
            //Desired rotation, updated every (fixed) frame
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(direction.x, 0, direction.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            if (ground)
            {
                if (Mathf.Abs(deltaAngle) < 90)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);

                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * sharpRotationDamp);
                }
            }
            else transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * airRotation);
        }
    }

    public virtual void AttackRotation()
    {
        //if (strafe && !sprinting && ground)
        //{
        //    if (strafeTarget == null) return;
        //    Vector3 desiredDirection = strafeTarget.position - transform.position;
        //    Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
        //    deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);


        //    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * sharpRotationDamp);

        //    return;
        //}

        if (direction != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(direction.x, 0, direction.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            transform.rotation = desiredRotation;
        }
    }

    public virtual void MovementProperties()
    {
        status.regenStamina = forcedWalk && sprinting && ground || !sprinting && ground || !isMoving && ground;

        if (!isMoving)
        {
            currentVel = 0;
            // actualVelocity = Mathf.SmoothDamp(actualVelocity, currentVel, ref zeroFloat, smoothDeacceleration);
        }
        else if (isMoving && ground)
        {

            if (status.currentState == Status.State.TopAnimation)
            {
                run = false;
                sprinting = false;
                currentVel = walkSpeed;
            }
            else
            {
                if (direction.magnitude > walkThreshold) { run = true; }
                else { run = false; }

                if (sprinting && !forcedWalk)
                {
                    currentVel = sprintSpeed * status.currentStats.movementSpeedModifier;
                    if (status.currentStats.currentStamina > 0)
                    {
                        sprintCounter--;
                        if (sprintCounter <= 0 && ground)
                        {
                            sprintCounter = sprintCostTimer;
                            status.currentStats.currentStamina -= 1;
                        }


                    }
                    //  else { sprinting = false; }
                }
                else if (run && !forcedWalk)
                {
                    if (strafe)
                        currentVel = strafeSpeed * status.currentStats.movementSpeedModifier;
                    else
                        currentVel = runSpeed * status.currentStats.movementSpeedModifier;
                }
                else
                {
                    currentVel = walkSpeed;
                }
            }
        }

        if (currentVel > actualVelocity)
            actualVelocity = Mathf.SmoothDamp(actualVelocity, currentVel, ref zeroFloat, smoothAcceleration);
        else if (currentVel < actualVelocity)
            actualVelocity = Mathf.SmoothDamp(actualVelocity, currentVel, ref zeroFloat, smoothDeacceleration);
        //if (currentVel > actualVelocity)
        //    actualVelocity += smoothAcceleration;
        //else if (currentVel < actualVelocity)
        //    actualVelocity -= smoothDeacceleration;
    }

    public void Jump()
    {
        if (status.currentStats.currentStamina <= 0) return;
        jumpEvent?.Invoke();

        //if(
        //isMoving = true;
        //PlayerMovement();


        status.currentStats.currentStamina -= 10;
        jumpCounter = minimumJumpTime;
        col.material = airMat;
        ground = false;



        if (isMoving)
        {
            if (strafe)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(direction.x, 0, direction.z), Vector3.up), 0);
                deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
                transform.rotation = desiredRotation;
                // rb.velocity = new Vector3(direction.x * currentVel, rb.velocity.y, direction.z * currentVel);
            }
            if (currentVel == 0)
                rb.velocity = new Vector3(transform.forward.x * walkSpeed, rb.velocity.y, transform.forward.z * walkSpeed);
            else
                rb.velocity = new Vector3(transform.forward.x * currentVel, rb.velocity.y, transform.forward.z * currentVel);
        }


        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }
    public void DoubleJump()
    {
        status.currentStats.currentStamina -= 10;
        jumpCounter = minimumJumpTime;
        col.material = airMat;
        ground = false;
        doubleJumpEvent?.Invoke();

        if (isMoving)
        {
            if (currentVel == 0)
                rb.velocity = new Vector3(transform.forward.x * walkSpeed, rb.velocity.y, transform.forward.z * walkSpeed);
            else
                rb.velocity = new Vector3(transform.forward.x * currentVel, rb.velocity.y, transform.forward.z * currentVel);
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    public bool GroundDetection()
    {
        check = Physics.Raycast(transform.position + Vector3.up * 0.1F, Vector3.down, out hit, rayLength, groundMask);
        check2 = Physics.Raycast(transform.position + Vector3.up * 0.1F + transform.forward * offset, Vector3.down, out hit2, rayLength, groundMask);
        Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.down * rayLength, Color.yellow);
        Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up * 0.1F + transform.forward * offset, Vector3.down * rayLength, Color.yellow);
        Debug.DrawRay(hit2.point, hit2.normal * 2f, Color.blue);
        if (jumpCounter > 0)
        {
            jumpCounter--;
            return false;
        }

        if (check)
        {
            float angle = Vector3.Angle(hit2.normal, Vector3.up);
            if (angle > stepAngle) check = false;
        }
        if (check2)
        {
            float angle2 = Vector3.Angle(hit2.normal, Vector3.up);
            if (angle2 > stepAngle) check2 = false;
        }



        if (!ground)
        {
            if (check || check2)
            {
                if (rb.velocity.y <= 0)
                {
                    landEvent?.Invoke();
                    ground = true;
                }
            }
        }


        if (stairs && isMoving) col.material = airMat;
        else if (ground) col.material = groundMat;
        else col.material = airMat;
        return ground;
    }

    public void PlayerMovement()
    {

        //StepHeight();
        rb.useGravity = !stairs;
        if (!ground)
        //Airborne
        {
            Vector3 temp = rb.velocity;
            temp.y = 0;
            if (!isMoving)
            {
                rb.velocity = transform.forward * temp.magnitude * airDeceleration + rb.velocity.y * Vector3.up;
            }
            else
                rb.velocity = transform.forward * temp.magnitude + rb.velocity.y * Vector3.up;
        }
        else if (forwardOnly || sprinting)
        {
            //Running
            rb.velocity = new Vector3(transform.forward.x * actualVelocity, rb.velocity.y, transform.forward.z * actualVelocity);
        }
        else
        //Normal Walking
        {
            Vector3 temp = direction.normalized;
            if (stairs && isMoving)
            {
                rb.velocity = new Vector3(transform.forward.x * actualVelocity, stepHeight, transform.forward.z * actualVelocity);
            }
            else
            {
                if (check2)
                {
                    Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit2.normal) * 3, Color.red);
                    rb.velocity = Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit2.normal) * actualVelocity;
                }

                else if (check)
                {
                    Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit.normal) * 3, Color.red);
                    // rb.velocity = new Vector3(transform.forward.x * actualVelocity, rb.velocity.y, transform.forward.z * actualVelocity);
                    rb.velocity = Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit.normal) * actualVelocity;
                }
                else rb.velocity = new Vector3(transform.forward.x * actualVelocity, rb.velocity.y, transform.forward.z * actualVelocity);
            }
        }

    }

    private void StepHeight()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.1F, transform.forward * stepRayLength, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * stepHeight, transform.forward * stepRayLength, Color.magenta);
        bool foundStairs = false;
        if (isMoving)
        {
            RaycastHit tempHit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.02F, transform.forward, out tempHit, stepRayLength, groundMask))
            {
                float angle = Vector3.Angle(tempHit.normal, Vector3.up);
                //Debug.Log(angle);
                //if (angle > stepAngle)
                //{
                //    return;
                //}
                if (!Physics.Raycast(transform.position + Vector3.up * stepHeight, transform.forward, stepRayLength, groundMask))
                {
                    foundStairs = true;
                    Debug.Log("Step Height");
                    //col.material = airMat;
                    rb.velocity += Vector3.up * stepSmooth;
                }
            }
            stairs = foundStairs;
        }
        if (!foundStairs && stairs && !isMoving)
        {
            rb.velocity = Vector3.zero;
            //RaycastHit tempHit;
            //if (!Physics.Raycast(transform.position, -transform.up, out tempHit, rayLength * 2, groundMask))
            //{
            //    stairs = false;
            //}
        }

    }
    #region Math

    public Vector3 RelativeToForward()
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        Vector3 temp = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        if (!isMoving) temp = Vector3.zero;
        return temp;
    }

    public Vector3 RemoveAxis(Vector3 vec, Vector3 removedAxis)
    {
        Vector3 n = removedAxis;
        Vector3 dir = vec;

        float d = Vector3.Dot(dir, n);


        return n * d;
    }

    public Vector3 RemoveYAxis(Vector3 vec)
    {
        Vector3 n = Vector3.down;

        Vector3 dir = vec;
        float d = Vector3.Dot(dir, n);
        dir -= n * d;
        return dir;
    }
    #endregion
}
