using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public Transform player;
    private NavMeshAgent navMeshAgent;
    public State currentState = State.Idle;
    public Transform target;
    [SerializeField] private float sightRange;

    public LayerMask whatIsPlayer;

    [Header("Patroling")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject meleeAttack;

    [Header("Attack Ranges")]
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float rangedAttackRange = 12f;

    [Header("Projectile Attack")]
    public GameObject projectile;
    public float marginOfError = 1.5f;
    public float projectileSpeed = 25;
    Vector3 targetLastPosition;
    public float interval = 5;
    public float timer;

    [Header("States")]
    public bool playerInSightRange, playerInAttackRange;
    [SerializeField] private bool isRanged = false;
    [SerializeField] private bool isDamagable = true;

    [Header("NavMesh Checks")]
    [SerializeField] private float navMeshSnapTolerance = 0.25f;
    private bool playerOnNavMesh;

    // ignore reset when player is temporarily airborne
    [SerializeField] private float airborneGraceTime = 0.35f;
    private float _airborneGraceTimer;

    // raycast settings to find ground under player
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float playerGroundRayDistance = 6f;

    [Header("Ranged Arc Options")]
    [SerializeField] private bool projectileUseArc = false; // Toggle arc shots
    [SerializeField] private float projectileArcHeight = 3f;

    private NavMeshPath _path;

    private Vector3 _resetPosition;       
    private bool _hasResetPosition;       
    private bool _wasPlayerInSightRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();

        target = player; // Ensure target is assigned
        _path = new NavMeshPath(); // Cache path object
    }

    void Update()
    {
        DetectPlayer();

        // capture "home" position at the moment we first spot the player
        if (playerInSightRange && !_wasPlayerInSightRange) 
        {
            _resetPosition = transform.position;           
            _hasResetPosition = true;                      
        }
        _wasPlayerInSightRange = playerInSightRange;       

        // if player is in sight but off NavMesh, force Reset
        if (playerInSightRange && !playerOnNavMesh)
        {
            currentState = State.Reset;
        }
        else
        {
            if (!playerInSightRange && !playerInAttackRange)
                currentState = State.Patrol;
            else if (playerInSightRange && !playerInAttackRange)
                currentState = State.Chase;
            else if (playerInSightRange && playerInAttackRange)
                currentState = State.Attack;
        }

        switch (currentState)
        {
            case State.Idle:
                Debug.Log("Waiting...");
                break;

            case State.Patrol:
                if (!walkPointSet) SearchWalkPoint();
                if (walkPointSet) navMeshAgent.SetDestination(walkPoint);

                if ((transform.position - walkPoint).magnitude < 1f)
                    walkPointSet = false;

                Debug.Log("Patroling...");
                break;

            case State.Chase:
                if (!TrySetDestination(player.position))
                {
                    currentState = State.Reset;
                    break;
                }

                Debug.Log("Chasing!");
                break;

            case State.Attack:
                transform.LookAt(player);

                // If player goes off-mesh during attack, reset
                if (!playerOnNavMesh)
                {
                    currentState = State.Reset;
                    break;
                }

                // If ranged and player exits attack range, chase again
                if (isRanged && !playerInAttackRange)
                {
                    currentState = State.Chase;
                    break;
                }

                if (isRanged)
                {
                    navMeshAgent.ResetPath(); // Stop moving while firing
                    HandleRangedAttack();
                }
                else
                {
                    if (!alreadyAttacked)
                    {
                        Instantiate(meleeAttack, transform.position + transform.forward, Quaternion.identity);
                        alreadyAttacked = true;
                        Invoke(nameof(ResetAttack), timeBetweenAttacks);
                    }
                }

                Debug.Log("Attacking!");
                break;

            case State.Reset:
                isDamagable = false;

                // go back to the position we were at when we first spotted the player
                if (_hasResetPosition) 
                {
                    // Snap reset position onto navmesh just in case
                    if (NavMesh.SamplePosition(_resetPosition, out NavMeshHit homeHit, 2.0f, NavMesh.AllAreas)) 
                    {
                        navMeshAgent.SetDestination(homeHit.position); 

                        // once we arrive, resume patrol and clear aggro memory
                        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.75f) 
                        {
                            _hasResetPosition = false;      
                            _wasPlayerInSightRange = false; 
                            currentState = State.Patrol;    
                        }
                    }
                    else
                    {
                        // If we can't find navmesh near home, just patrol
                        _hasResetPosition = false; 
                        currentState = State.Patrol; 
                    }
                }
                else
                {
                    // Fallback behavior (no stored reset point)
                    navMeshAgent.ResetPath();
                    currentState = State.Patrol;
                }

                Debug.Log("Run Away!");
                break;
        }
    }

    float GetAttackRange()
    {
        return isRanged ? rangedAttackRange : meleeAttackRange;
    }

    // handles ranged timer + fire
    void HandleRangedAttack()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            timer = 0f;
            Vector3 targetPosition = player.position + (Trajectory() * TimeToReach());
            FireProjectile(targetPosition);
        }
    }

    // sets destination only if reachable on navmesh
    bool TrySetDestination(Vector3 rawTargetPosition)
    {
        // Snap the target onto the NavMesh (if possible)
        if (!NavMesh.SamplePosition(rawTargetPosition, out NavMeshHit hit, navMeshSnapTolerance, NavMesh.AllAreas))
            return false; // player likely off-mesh / unreachable

        // Verify path is complete before committing
        if (!navMeshAgent.CalculatePath(hit.position, _path))
            return false;

        if (_path.status != NavMeshPathStatus.PathComplete)
            return false;

        navMeshAgent.SetDestination(hit.position);
        return true;
    }

    float TimeToReach()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance / projectileSpeed;
    }

    void ResetTimer()
    {
        timer -= interval;
        interval = Random.Range(1f, 4f);
    }

    Vector3 Trajectory()
    {
        Vector3 direction = target.position - targetLastPosition;
        targetLastPosition = target.position;
        Vector3 inaccuracy = new(
            Random.Range(-marginOfError, marginOfError),
            Random.Range(-marginOfError, marginOfError),
            Random.Range(-marginOfError, marginOfError)
        );
        return direction + inaccuracy;
    }

    void FireProjectile(Vector3 targetPosition)
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        EnemyProjectile ep = newProjectile.GetComponent<EnemyProjectile>();
        ep.Init(targetPosition, projectileSpeed, projectileUseArc, projectileArcHeight);
    }

    Vector3 AimError(float amount)
    {
        return Random.insideUnitSphere * amount;
    }

    void DetectPlayer()
    {
        playerInSightRange =
        Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        float attackRange = GetAttackRange();
        playerInAttackRange =
            Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Check NavMesh under the player instead of at player's current (possibly airborne) position
        Vector3 navCheckPoint = player.position;

        if (Physics.Raycast(player.position + Vector3.up * 0.5f, Vector3.down,
            out RaycastHit groundHit, playerGroundRayDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            navCheckPoint = groundHit.point;
            _airborneGraceTimer = airborneGraceTime; // player has ground under them, refresh grace
        }
        else
        {
            // likely airborne (jumping/falling), count down grace
            _airborneGraceTimer -= Time.deltaTime;
        }

        // allow a grace window where we treat player as "reachable"
        if (_airborneGraceTimer > 0f)
        {
            playerOnNavMesh = true;
        }
        else
        {
            playerOnNavMesh = NavMesh.SamplePosition(navCheckPoint, out _, navMeshSnapTolerance, NavMesh.AllAreas);
        }
    }

    private void SearchWalkPoint()
    {
        // Try a few times to find a valid point on the NavMesh within range
        const int maxAttempts = 12;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random point around the enemy (in XZ)
            Vector2 randomCircle = Random.insideUnitCircle * walkPointRange; 
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y); 

            // Snap it onto the NavMesh (within a small tolerance)
            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                continue;

            // Verify a complete path exists to that point
            if (!navMeshAgent.CalculatePath(hit.position, _path))
                continue;

            if (_path.status != NavMeshPathStatus.PathComplete)
                continue;

            walkPoint = hit.position; 
            walkPointSet = true;      
            return;                   
        }

        // If it fails it will try again next frame
        walkPointSet = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    [System.Serializable]
    public enum State { Idle, Patrol, Chase, Attack, Reset }
}

