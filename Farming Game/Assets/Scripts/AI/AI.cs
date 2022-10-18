using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AI : MonoBehaviour
{
    public enum State { Idle, Move, Alert, Attack, Passive };
    public State currentState = State.Idle;

    private NavMeshPath path;
    [Space(10)]
    [HeaderAttribute("Field of View")]
    [FoldoutGroup("Field of view")] [SerializeField] public float range;
    [FoldoutGroup("Field of view")] [Range(0, 360)] public float viewAngle;
    [FoldoutGroup("Field of view")] [SerializeField] LayerMask mask;
    RaycastHit hit;

    [FoldoutGroup("Debug")] public Vector3 directionVector;
    int reachedCorner;
    [FoldoutGroup("Pathfinding")] [HideInInspector] public float cornerMinDistance;
    [FoldoutGroup("Pathfinding")] public float minDistance;
    private bool hasValidPath;
    [FoldoutGroup("Pathfinding")] private float elapsed = 0.0f;
    [FoldoutGroup("Pathfinding")] public float pathUpdateTime;
    public delegate void AIEvent();
    [FoldoutGroup("Pathfinding")] public AIEvent detectEvent;
    [FoldoutGroup("Pathfinding")] public float detectionDelay = 0.5F;
    [FoldoutGroup("Pathfinding")] public float detectionSpread = 5F;
    [FoldoutGroup("Pathfinding")] public LayerMask enemyMask;
    public bool isWalking;

    protected Movement movement;
    protected bool run;

    public float stoppingDistance = 4;
    public float enemyDetectionRadius = 8;
    private bool isClose;

    [FoldoutGroup("Patrol")] public bool willPatrol;
    [FoldoutGroup("Patrol")] public float patrolRange;
    [FoldoutGroup("Patrol")] public float patrolInterval;
    [FoldoutGroup("Patrol")] public Transform[] patrolPoints;
    [FoldoutGroup("Patrol")] public int patrolID;

    Vector3 startPosition;



    protected Status status;
    protected AttackScript attack;
    protected AIManager manager;

    [FoldoutGroup("Attack")] public float cooldown = 0.5f;


    bool detectOnce;
   public bool detected;

    [FoldoutGroup("Attack")] public float attackDistance = 4;
    protected float lastAttackTime;
    [FoldoutGroup("Attack")] public bool inCooldown;
    [FoldoutGroup("Debug")] public Transform target;
    [FoldoutGroup("Debug")] public Transform currentTarget;
    [FoldoutGroup("Debug")] public List<Move> attackQueue;

    [FoldoutGroup("Setup")] [SerializeField] private float yOffset = 0.5F;
    [FoldoutGroup("Debug")] public bool clearLine;
    [FoldoutGroup("Debug")] public bool withinAngle;
    [FoldoutGroup("Debug")] public bool inRange;

    protected void Awake()
    {
        //target = points[0];
        status = GetComponent<Status>();
        attack = GetComponent<AttackScript>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = target;

        currentState = State.Idle;

        status.deathEvent += RemoveFromAIManager;
    }


    void RemoveFromAIManager()
    {
        if (AIManager.Instance.allEnemies.Contains(this))
            AIManager.Instance.allEnemies.Remove(this);

        if (AIManager.Instance.activeEnemies.Contains(this))
            AIManager.Instance.activeEnemies.Remove(this);

        Destroy(this);
    }



    private void FixedUpdate()
    {
        StateMachine();
        ResolveCooldown();

        clearLine = ClearLine();
        withinAngle = WithinAngle();
        inRange = TargetInRange();

    }

    void ResolveCooldown()
    {
        if (inCooldown && currentState != State.Attack)
            if (lastAttackTime + cooldown < Time.time)
            {
                inCooldown = false;
            }
    }

    public virtual void StateMachine()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Move:
                Move();
                break;
            case State.Alert:
                Alert();
                break;
            case State.Attack:
                Attacking();
                break;
        }
    }

    protected virtual void Alert()
    {
        //Move();


        if (status.currentState == Status.State.InAnimation)
        {
            movement.direction = FindPath();
        }
        else

            movement.direction = Vector3.zero;

        if (!inCooldown)
        {
            currentState = State.Attack;
        }
    }

    void Attacking()
    {
        currentTarget = target;
        movement.direction = FindPath();
        if (attackQueue.Count > 0)
        {
            if (attack.attackString || status.currentState == Status.State.Neutral)
            {
                movement.direction = FindPath();
                AttackQueue();
            }
        }
        else if (inCooldown && status.currentState != Status.State.InAnimation)
        {
            lastAttackTime = Time.time;
            currentState = State.Alert;
        }
        else if (status.currentState != Status.State.InAnimation)
        {
            Move();
            if (!inCooldown && InLineOfSight())
            {
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    movement.direction = FindPath();
                    int rng = Random.Range(0, attack.moveset.combos.Length);
                    inCooldown = true;
                    for (int i = 0; i < attack.moveset.combos[rng].moves.Length; i++)
                    {
                        attackQueue.Add(attack.moveset.combos[rng].moves[i]);
                    }
                   
                }
            }
        }
    }

    protected void AttackQueue()
    {
        attack.Attack(attackQueue[0]);
        attackQueue.RemoveAt(0);
    }



    protected void Start()
    {
        movement = GetComponent<Movement>();
        status.healthEvent += Detect;
        path = new NavMeshPath();
        elapsed = pathUpdateTime;
        startPosition = transform.position;
        AIManager.Instance.allEnemies.Add(this);

    }

    protected void Update()
    {
        DetectionEvent();

        if (!detected) return;

        FindPath();
    }

    void CalculatePath()
    {
        elapsed += Time.deltaTime;
        if (elapsed > pathUpdateTime && currentTarget != null)
        {
            reachedCorner = 0;
            elapsed -= pathUpdateTime;
            NavMesh.CalculatePath(transform.position, currentTarget.transform.position, NavMesh.AllAreas, path);
        }
    }

    void CalculatePath(Vector3 v)
    {
        elapsed += Time.deltaTime;
        if (elapsed > pathUpdateTime)
        {
            reachedCorner = 0;
            elapsed -= pathUpdateTime;
            NavMesh.CalculatePath(transform.position, v, NavMesh.AllAreas, path);
        }
    }


    public void DebugPath()
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.blue);
    }

    public Vector3 FindPath()
    {
        CalculatePath();
        DebugPath();

        if (path.corners.Length > 1)
        {
            directionVector = (path.corners[reachedCorner + 1] - path.corners[reachedCorner]).normalized;
            movement.isMoving = true;
            if (isWalking) directionVector = directionVector * 0.5F;

            if (Vector3.Distance(transform.position, path.corners[reachedCorner + 1]) < cornerMinDistance)
            {
                hasValidPath = false;
                if (path.corners.Length > reachedCorner + 2)
                    reachedCorner++;
            }
            else
            {
                hasValidPath = true;
            }
        }
        return directionVector;
    }

    public Vector3 FindPath(Vector3 v)
    {
        CalculatePath(v);
        DebugPath();

        if (path.corners.Length > 1)
        {
            directionVector = (path.corners[reachedCorner + 1] - path.corners[reachedCorner]).normalized;

            if (isWalking) directionVector = directionVector * 0.5F;

            if (Vector3.Distance(transform.position, path.corners[reachedCorner + 1]) < cornerMinDistance)
            {
                hasValidPath = false;
                if (path.corners.Length > reachedCorner + 2)
                    reachedCorner++;
            }
            else
            {
                hasValidPath = true;
            }
        }
        return directionVector;
    }

    //public Transform GetClosestEnemy()
    //{
    //    float minDistance = float.PositiveInfinity;
    //    GameObject closestEnemy = null;
    //    foreach (AI enemy in AIManager.allEnemies)
    //    {
    //        float distance = Vector3.Distance(enemy.transform.position, transform.position);
    //        if (distance < minDistance)
    //        {
    //            minDistance = distance;
    //            closestEnemy = enemy.gameObject;
    //        }
    //    }
    //    if (AIManager.allEnemies.Count < 1)
    //        return null;
    //    else
    //        return closestEnemy.transform;
    //}


    public void DetectionEvent()
    {
        if (!detectOnce)
        {
            if (TargetInRange() && InLineOfSight())
            {
                Detect();
            }
        }
    }

    void Detect()
    {
        if (detectOnce) return;
        currentTarget = target;
        detectOnce = true;
        detectEvent?.Invoke();
        if (!AIManager.Instance.activeEnemies.Contains(this))
        {
            AIManager.Instance.activeEnemies.Add(this);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionSpread, enemyMask);
            foreach (var item in hitColliders)
            {

                AI ai = item.GetComponent<AI>();
                if(ai != null)
                ai.Detect();

            }
        }
        StartCoroutine("DelayDetection");
    }

    IEnumerator DelayDetection()
    {
        yield return new WaitForSeconds(detectionDelay);
        detected = true;
    }

    protected void Move()
    {
        if (target != null)
        {

            if (Vector3.Distance(transform.position, target.transform.position) > stoppingDistance)
            {
                movement.strafe = false;
                movement.forwardOnly = true;
                movement.direction = FindPath();
            }
            else
            {
                if (WithinAngle())
                    Idle();
                else
                {
                    movement.strafeTarget = target;
                    movement.strafe = true;
                    movement.forwardOnly = false;
                    movement.RotateInPlace((target.position - transform.position).normalized);
                }
            }
        }
        else
        {
            Idle();
        }
    }
    protected void Flee()
    {
        if (target != null)
        {
            movement.direction -= FindPath();
        }
        else
        {
            Idle();
        }
    }

    protected void Idle()
    {
        if (!willPatrol || detected)
        {
            movement.direction = new Vector3(0, 0, 0);

            if (detected && !inCooldown)
            {

                currentState = State.Attack;
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        currentTarget = patrolPoints[patrolID];
        if (Vector3.Distance(transform.position, patrolPoints[patrolID].position) < 2)
        {
            patrolID++;
            if (patrolID >= patrolPoints.Length) patrolID = 0;
        }
        movement.direction  = FindPath() * 0.3F;
    }

    void Dying()
    {
        status.Death();
    }
    protected void OnDisable()
    {
        if (AIManager.Instance.allEnemies.Contains(this))
            AIManager.Instance.allEnemies.Remove(this);
    }

    public bool TargetInRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) < range;
    }

    public Quaternion TargetDirection()
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        return rotation;
    }
    public Quaternion TargetDirection(Transform origin)
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - origin.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        return rotation;
    }


    public bool WithinAngle()
    {
        bool withinAngle = (Vector3.Angle(transform.forward, TargetDirectionIgnoreTilt()) < viewAngle / 2);
        return withinAngle;
    }

    public bool CheckTargetAngle()
    {
        bool withinAngle = (Vector3.Angle(target.transform.forward, TargetDirectionIgnoreTilt()) < viewAngle / 2);
        return withinAngle;
    }

    public bool ClearLine()
    {
        bool clearLine = Physics.Raycast(transform.position + Vector3.up * yOffset, ((target.transform.position + Vector3.up) - (transform.position + Vector3.up * yOffset)).normalized, out hit, 1000, mask);
        Debug.DrawLine(transform.position + Vector3.up * yOffset, hit.point, Color.yellow);
        return clearLine;
    }

    public bool InLineOfSight()
    {
        bool seePlayer = ClearLine() && WithinAngle();
        return seePlayer;
    }

    public Vector3 TargetDirectionVector()
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - transform.position;
        return relativePos.normalized;
    }

    public Vector3 TargetDirectionIgnoreTilt()
    {
        Vector3 relativePos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        return relativePos.normalized;
    }

    public Vector3 TargetDirectionIgnoreTilt(Vector3 temp)
    {
        Vector3 relativePos = new Vector3(temp.x, transform.position.y, temp.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        return relativePos.normalized;
    }


    public Vector3 AngleToVector(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
