using System.Collections;
using UnityEngine;

/// <summary>
/// Master enemy script. Drives physics-based movement, spacing, targeting, and attack pacing.
/// Attach to the Enemy root object (the one with the Rigidbody).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CreatureDefs : MonoBehaviour, IDamageable
{
    public enum RoamMode { Waypoints, RandomRoam }
    public enum AttackMode { Melee, ProjectileStraight, ProjectileArc, Charger }

    [Header("References")]
    [Tooltip("Optional. If empty, will auto-find the Player by tag.")]
    [SerializeField] private Transform target;

    [Tooltip("Where attacks/projectiles originate from (ex: your enemy's attackPt transform).")]
    [SerializeField] private Transform attackPoint;

    [Tooltip("Non-trigger collider used for physical collisions (so enemies can bump/push and be pushed).")]
    [SerializeField] private Collider physicsCollider;

    [Tooltip("Enemy layer mask for separation (anti-clump). Typically set to the Enemy layer only.")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Core Stats")]
    [Tooltip("Max health for this enemy.")]
    [SerializeField, Min(1f)] private float maxHealth = 30f;

    [Header("Movement")]
    [Tooltip("Minimum desired horizontal move speed.")]
    [SerializeField, Min(0f)] private float minSpeed = 2f;

    [Tooltip("Maximum desired horizontal move speed.")]
    [SerializeField, Min(0f)] private float maxSpeed = 4f;

    [Tooltip("Max horizontal acceleration (m/s^2) applied while steering.")]
    [SerializeField, Min(0f)] private float maxAcceleration = 25f;

    [Tooltip("How quickly the enemy rotates to face target/movement (degrees/sec).")]
    [SerializeField, Min(0f)] private float turnSpeedDegPerSec = 720f;

    [Tooltip("Preferred combat distance for ranged styles (meters). Melee uses it to back away and wait for their next allowed attack.")]
    [SerializeField, Min(0f)] private float HoldDistance = 6f;

    [Tooltip("Stop steering briefly after being hit/knocked back (seconds) so physics actually 'wins'.")]
    [SerializeField, Min(0f)] private float controlLockSecondsOnHit = 0.15f;

    [Header("Roaming")]
    [SerializeField] private RoamMode roamMode = RoamMode.RandomRoam;

    [Tooltip("Optional waypoint list (used when Roam Mode = Waypoints).")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("Random roam radius around the enemy's spawn point (meters).")]
    [SerializeField, Min(0f)] private float roamRadius = 10f;

    [Tooltip("How close we need to be to a roam/waypoint target before picking the next one (meters).")]
    [SerializeField, Min(0f)] private float roamArriveDistance = 1.0f;

    [Tooltip("Seconds to wait after reaching a roam point before picking the next one.")]
    [SerializeField, Min(0f)] private float roamWaitSeconds = 0.25f;

    [Header("Detection")]
    [Tooltip("Distance to start chasing the player (meters).")]
    [SerializeField, Min(0f)] private float aggroDistance = 14f;

    [Tooltip("Distance to stop chasing after losing aggro (meters). Use > aggroDistance for hysteresis.")]
    [SerializeField, Min(0f)] private float deaggroDistance = 18f;

    [Header("Attacking")]
    [SerializeField] private AttackMode attackMode = AttackMode.Melee;

    [Tooltip("Distance required to attack (meters). For ranged, this is max attack distance.")]
    [SerializeField, Min(0f)] private float attackRange = 2.2f;

    [Tooltip("Seconds between attacks for this enemy.")]
    [SerializeField, Min(0f)] private float attackCooldownSeconds = 1.2f;

    [Tooltip("Wind-up time before the attack fires (seconds).")]
    [SerializeField, Min(0f)] private float attackWindupSeconds = 0.25f;

    [Tooltip("When using melee hitboxes: how long the hitbox stays enabled (seconds).")]
    [SerializeField, Min(0f)] private float meleeActiveSeconds = 0.2f;

    [Tooltip("Optional melee collider (AttackCollider child). Will be enabled only during meleeActiveSeconds.")]
    [SerializeField] private Collider meleeHitbox;

    [Tooltip("Projectile prefab with a Rigidbody (for ProjectileStraight/ProjectileArc).")]
    [SerializeField] private Rigidbody projectilePrefab;

    [Tooltip("Projectile speed for straight shots (m/s).")]
    [SerializeField, Min(0f)] private float projectileSpeed = 14f;

    [Tooltip("Speed that chargers launch at the player (m/s).")]
    [SerializeField, Min(0f)] private float chargeSpeed = 14f;

    [Tooltip("For arc shots: extra height of the arc apex above the higher of start/target (meters).")]
    [SerializeField, Min(0f)] private float arcApexHeight = 3f;

    [Header("Anti-Clump")]
    [Tooltip("Radius to look for nearby enemies to push away from.")]
    [SerializeField, Min(0f)] private float separationRadius = 1.2f;

    [Tooltip("Strength of separation acceleration (m/s^2).")]
    [SerializeField, Min(0f)] private float separationStrength = 20f;

    [Tooltip("Max neighbors to consider for separation (performance cap).")]
    [SerializeField, Range(1, 64)] private int maxSeparationNeighbors = 16;

    [Header("Attack Director")]
    [Tooltip("If enabled, this enemy must be granted permission by EnemyAttackDirector before attacking.")]
    [SerializeField] private bool useAttackDirector = true;

    [Tooltip("How often this enemy is allowed to ask the director for an attack slot (seconds).")]
    [SerializeField, Min(0f)] private float directorRequestInterval = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    private Rigidbody _rb;
    private float _health;
    private Vector3 _spawnPos;

    private bool _hasAggro;
    private float _nextAttackTime;
    private bool _isAttacking;

    // Roam
    private Vector3 _roamTarget;
    private bool _hasRoamTarget;
    private float _nextRoamPickTime;
    private int _waypointIndex;

    // Spacing
    private readonly Collider[] _neighborBuffer = new Collider[64];

    // Status effects
    private float _controlLockUntil;
    private float _slipUntil;
    private float _slipSteerMultiplier = 1f;

    // Attack pacing
    private EnemyAttackDirector _director;
    private float _nextDirectorRequestTime;
    private int _orbitSign;

    private const string DefaultPlayerTag = "PlayerHitPt";

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _health = maxHealth;
        _spawnPos = transform.position;

        _orbitSign = (Random.value < 0.5f) ? -1 : 1;

        if (useAttackDirector)
            _director = EnemyAttackDirector.Instance;

        if (!target)
            TryFindTargetByTag(DefaultPlayerTag);

        // Safety: if you forgot to assign a non-trigger collider, we'll try to find one.
        if (!physicsCollider)
            physicsCollider = GetComponentInChildren<Collider>();

        // Start with melee hitbox off.
        if (meleeHitbox)
            meleeHitbox.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!target)
            TryFindTargetByTag(DefaultPlayerTag);

        UpdateAggroState();

        if (_isAttacking)
        {
            ApplyFacing(toTarget: true);
            return;
        }

        Vector3 desiredDir;
        float desiredSpeed;

        if (_hasAggro && target)
        {
            CombatMove(out desiredDir, out desiredSpeed);

            float sqrDist = HorizontalSqrDistance(transform.position, target.position);
            bool inRange = sqrDist <= attackRange * attackRange;

            if (inRange && Time.time >= _nextAttackTime)
                TryStartAttack(sqrDist);
        }
        else
        {
            RoamMove(out desiredDir, out desiredSpeed);
        }

        ApplySteering(desiredDir, desiredSpeed);
        ApplySeparation();
        ApplyFacing(toTarget: _hasAggro);
    }

    private void UpdateAggroState()
    {
        if (!target) { _hasAggro = false; return; }

        float sqrDist = HorizontalSqrDistance(transform.position, target.position);
        float aggroSqr = aggroDistance * aggroDistance;
        float deaggroSqr = deaggroDistance * deaggroDistance;

        if (!_hasAggro)
        {
            if (sqrDist <= aggroSqr) _hasAggro = true;
        }
        else
        {
            if (sqrDist >= deaggroSqr) _hasAggro = false;
        }
    }

    private void CombatMove(out Vector3 desiredDir, out float desiredSpeed)
    {
        Vector3 toTarget = target.position - transform.position;
        Vector3 toTargetFlat = Vector3.ProjectOnPlane(toTarget, Vector3.up);

        float dist = toTargetFlat.magnitude;
        Vector3 dirToTarget = (dist > 0.001f) ? (toTargetFlat / dist) : Vector3.forward;

        float steerMultiplier = GetSteerMultiplier();

        if (attackMode == AttackMode.Melee)
        {
            // Has the melee enemy stay at a distance and circle the player until it gets permission to attack
            if (!_isAttacking)
            {
                Vector3 tangent2 = Vector3.Cross(Vector3.up, dirToTarget) * _orbitSign;
                desiredDir = (dirToTarget + tangent2 * 0.5f).normalized;
                desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, steerMultiplier);
                return;
            }
            desiredDir = dirToTarget;
            desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, steerMultiplier);
            return;
        }

        float hold = Mathf.Max(attackRange * 0.85f, HoldDistance);
        float deadBand = 0.6f;

        Vector3 radial;
        if (dist > hold + deadBand) radial = dirToTarget;
        else if (dist < hold - deadBand) radial = -dirToTarget;
        else radial = Vector3.zero;

        Vector3 tangent = Vector3.Cross(Vector3.up, dirToTarget) * _orbitSign;

        desiredDir = (radial + tangent * 0.65f).normalized;
        desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, steerMultiplier);
    }

    private void RoamMove(out Vector3 desiredDir, out float desiredSpeed)
    {
        desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.5f);

        if (Time.time < _nextRoamPickTime)
        {
            desiredDir = Vector3.zero;
            desiredSpeed = 0f;
            return;
        }

        if (roamMode == RoamMode.Waypoints && waypoints != null && waypoints.Length > 0)
        {
            Transform wp = waypoints[Mathf.Clamp(_waypointIndex, 0, waypoints.Length - 1)];
            _roamTarget = wp ? wp.position : _spawnPos;
            _hasRoamTarget = true;
        }
        else
        {
            if (!_hasRoamTarget)
                PickRandomRoamTarget();
        }

        Vector3 toPoint = _roamTarget - transform.position;
        Vector3 toPointFlat = Vector3.ProjectOnPlane(toPoint, Vector3.up);

        float dist = toPointFlat.magnitude;

        if (dist <= roamArriveDistance)
        {
            _nextRoamPickTime = Time.time + roamWaitSeconds;

            if (roamMode == RoamMode.Waypoints && waypoints != null && waypoints.Length > 0)
                _waypointIndex = (_waypointIndex + 1) % waypoints.Length;
            else
                _hasRoamTarget = false;

            desiredDir = Vector3.zero;
            desiredSpeed = 0f;
            return;
        }

        desiredDir = (dist > 0.001f) ? (toPointFlat / dist) : Vector3.zero;
        desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.35f);
    }

    private void PickRandomRoamTarget()
    {
        Vector2 r = Random.insideUnitCircle * roamRadius;
        _roamTarget = _spawnPos + new Vector3(r.x, 0f, r.y);
        _hasRoamTarget = true;
    }

    private void ApplySteering(Vector3 desiredDir, float desiredSpeed)
    {
        float steerMultiplier = GetSteerMultiplier();
        if (steerMultiplier <= 0.001f) return;

        Vector3 currentHoriz = Vector3.ProjectOnPlane(_rb.linearVelocity, Vector3.up);
        Vector3 desiredHoriz = desiredDir * desiredSpeed;

        Vector3 accel = (desiredHoriz - currentHoriz) / Time.fixedDeltaTime;
        accel = Vector3.ClampMagnitude(accel, maxAcceleration * steerMultiplier);

        _rb.AddForce(accel, ForceMode.Acceleration);
    }

    private void ApplySeparation()
    {
        if (separationRadius <= 0.001f || separationStrength <= 0.001f) return;

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            separationRadius,
            _neighborBuffer,
            enemyLayerMask,
            QueryTriggerInteraction.Collide);

        if (count <= 1) return;

        Vector3 push = Vector3.zero;
        int used = 0;

        for (int i = 0; i < count && used < maxSeparationNeighbors; i++)
        {
            Collider c = _neighborBuffer[i];
            if (!c) continue;

            Rigidbody otherRb = c.attachedRigidbody;
            if (!otherRb || otherRb == _rb) continue;

            Vector3 delta = transform.position - otherRb.position;
            delta = Vector3.ProjectOnPlane(delta, Vector3.up);

            float d = delta.magnitude;
            if (d < 0.0001f) continue;

            push += (delta / d) * (1f / Mathf.Max(d, 0.15f));
            used++;
        }

        if (used == 0) return;

        push = push.normalized * separationStrength;
        _rb.AddForce(push * GetSteerMultiplier(), ForceMode.Acceleration);
    }

    private void ApplyFacing(bool toTarget)
    {
        Vector3 faceDir;

        if (toTarget && target)
        {
            Vector3 to = target.position - transform.position;
            to = Vector3.ProjectOnPlane(to, Vector3.up);
            faceDir = (to.sqrMagnitude > 0.001f) ? to.normalized : transform.forward;
        }
        else
        {
            Vector3 v = Vector3.ProjectOnPlane(_rb.linearVelocity, Vector3.up);
            faceDir = (v.sqrMagnitude > 0.05f) ? v.normalized : transform.forward;
        }

        Quaternion desiredRot = Quaternion.LookRotation(faceDir, Vector3.up);
        Quaternion newRot = Quaternion.RotateTowards(_rb.rotation, desiredRot, turnSpeedDegPerSec * Time.fixedDeltaTime);
        _rb.MoveRotation(newRot);
    }

    private void TryStartAttack(float sqrDistToTarget)
    {
        if (_isAttacking) return;

        if (useAttackDirector && _director)
        {
            if (Time.time >= _nextDirectorRequestTime)
            {
                float dist = Mathf.Sqrt(Mathf.Max(0f, sqrDistToTarget));
                _director.ReportReadyToAttack(this, dist);
                _nextDirectorRequestTime = Time.time + directorRequestInterval;
            }

            if (!_director.CanStartAttack(this)) return;
            if (!_director.TryBeginAttack(this)) return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        if (attackWindupSeconds > 0f)
            yield return new WaitForSeconds(attackWindupSeconds);

        if (!target)
        {
            EndAttack();
            yield break;
        }

        switch (attackMode)
        {
            case AttackMode.Melee:
                yield return DoMeleeAttack();
                break;

            case AttackMode.ProjectileStraight:
                DoStraightProjectile();
                break;

            case AttackMode.ProjectileArc:
                DoArcProjectile();
                break;
            case AttackMode.Charger:
                yield return DoChargeAttack();
                break;
        }

        _nextAttackTime = Time.time + attackCooldownSeconds;
        EndAttack();
    }

    private IEnumerator DoMeleeAttack()
    {
        if (!meleeHitbox) yield break;

        EnemyAttacks ea = meleeHitbox.GetComponent<EnemyAttacks>();
        if (ea) ea.ResetPerAttackMemory();

        meleeHitbox.enabled = true;
        Debug.Log("Melee hitbox = " + meleeHitbox.enabled);
        if (meleeActiveSeconds > 0f) yield return new WaitForSeconds(meleeActiveSeconds);
        meleeHitbox.enabled = false;
        Debug.Log("Melee hitbox = " + meleeHitbox.enabled);
    }

    private void DoStraightProjectile()
    {
        if (!projectilePrefab) return;

        Transform origin = attackPoint ? attackPoint : transform;
        Rigidbody proj = Instantiate(projectilePrefab, origin.position, origin.rotation);

        Vector3 dir = (target.position - origin.position).normalized;
        proj.linearVelocity = dir * projectileSpeed;
    }

    private void DoArcProjectile()
    {
        if (!projectilePrefab) return;

        Transform origin = attackPoint ? attackPoint : transform;
        Rigidbody proj = Instantiate(projectilePrefab, origin.position, origin.rotation);

        Vector3 start = origin.position;
        Vector3 end = target.position;

        if (TryComputeBallisticVelocity(start, end, arcApexHeight, out Vector3 v))
            proj.linearVelocity = v;
        else
            proj.linearVelocity = (end - start).normalized * projectileSpeed;
    }

    // Target faces the enemy, waits for a short windup, then dashes forward in a straight line. Bonks into the player then retreats. Kinda funny.
    private IEnumerator DoChargeAttack()
    {
        if (!target) yield break;
        ApplySteering((target.position - transform.position).normalized, 0f);
        yield return new WaitForSeconds(attackWindupSeconds);
        Vector3 dir = (target.position - transform.position).normalized;
        _rb.AddForce(dir * chargeSpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
    }

    private static bool TryComputeBallisticVelocity(Vector3 start, Vector3 end, float apexExtraHeight, out Vector3 initialVelocity)
    {
        initialVelocity = Vector3.zero;

        float g = Mathf.Abs(Physics.gravity.y);
        if (g < 0.0001f) return false;

        Vector3 delta = end - start;
        Vector3 deltaXZ = Vector3.ProjectOnPlane(delta, Vector3.up);
        float xz = deltaXZ.magnitude;

        float apex = Mathf.Max(start.y, end.y) + Mathf.Max(0f, apexExtraHeight);

        float h1 = Mathf.Max(0.01f, apex - start.y);
        float h2 = Mathf.Max(0.01f, apex - end.y);

        float t1 = Mathf.Sqrt(2f * h1 / g);
        float t2 = Mathf.Sqrt(2f * h2 / g);
        float t = t1 + t2;

        if (t < 0.0001f) return false;

        float vy = g * t1;
        Vector3 vxz = (xz > 0.0001f) ? (deltaXZ / t) : Vector3.zero;

        initialVelocity = vxz + Vector3.up * vy;
        return true;
    }

    private void EndAttack()
    {
        _isAttacking = false;
        if (useAttackDirector && _director) _director.EndAttack(this);
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection, float impulseForce, GameObject instigator)
    {
        if (amount <= 0f) return;

        _health -= amount;

        if (controlLockSecondsOnHit > 0f)
            _controlLockUntil = Mathf.Max(_controlLockUntil, Time.time + controlLockSecondsOnHit);

        if (impulseForce > 0f)
        {
            Vector3 dir = hitDirection.normalized;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector3.up;
            _rb.AddForce(dir * impulseForce, ForceMode.Impulse);
        }

        if (_health <= 0f)
            Die();
    }

    public void ApplySlip(float durationSeconds, float steerMultiplier)
    {
        _slipUntil = Mathf.Max(_slipUntil, Time.time + Mathf.Max(0f, durationSeconds));
        _slipSteerMultiplier = Mathf.Clamp01(steerMultiplier);
    }

    private float GetSteerMultiplier()
    {
        if (Time.time < _controlLockUntil) return 0f;

        float mult = 1f;
        if (Time.time < _slipUntil) mult *= _slipSteerMultiplier;
        return mult;
    }

    private void Die()
    {
        //if (meleeHitbox) meleeHitbox.enabled = false;
        if (physicsCollider) physicsCollider.enabled = false;
        Destroy(gameObject);
    }

    private void TryFindTargetByTag(string tag)
    {
        GameObject go = GameObject.FindWithTag(tag);
        if (go) target = go.transform;
    }

    private static float HorizontalSqrDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return (a - b).sqrMagnitude;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}

