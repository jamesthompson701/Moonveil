using StarterAssets;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This SO will be the basis for every creature in the game.

public class CreatureDefs : MonoBehaviour
{
    [Header("States")]
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool isDamagable = true;
    public enum AIState { Idle, Patrol, Combat, Reset }
    public AIState currentState;

    [Header("References")]
    public Transform playerTransform;
    public Transform target;
    public NavMeshAgent navMeshAgent;
    public GameObject player;
    public LayerMask playerLayerMask;
    public float sightRange;
    public int attackDamage;
    public float maxHealth = 50;
    public float currentHealth;
    public float idleTimer;
    public float distanceToTarget;
    public Vector3[] wayPoints;
    public Vector3 _resetPosition;
    [Tooltip("Bool determines whether the AI used set waypoints or just wanders")]
    public bool isRoamingAI;
    public bool _hasResetPosition;
    public bool _wasPlayerInSightRange;
    public float playerHeight;
    public ThirdPersonController playerController;
    [Tooltip("Max height the player is allowed to be. If above the set amount the AI will reset. Adjust based on read playerHeight by the AI to prevent the AI from getting stuck in a state loop when the player is on a higher level or platform.")]
    public float playerMaxHeight;


    [Header("Patroling")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    [Tooltip("Max distance the AI look for a place to move. Adjust the navmesh stopping distance so they do not get stuck in a state loop")]
    public float walkPointRange;
    public bool targetFound;
    public int currentWaypoint;

    [Header("Attacking")]
    public float AttackInterval;
    public bool alreadyAttacked = false;
    public bool isAttacking = false;
    public GameObject attackObject;
    public float attackRange;
    public Transform attackPt;
    public float delay;
    [Tooltip("The angle at which the projectile will be launched.")]
    public float aimAngle = 45f;

    [Header("Projectile Attack")]
    [Tooltip("The amount of random inaccuracy applied to the projectile's trajectory. Keep below 1. I find 0.5 is enough")]
    public float marginOfError;
    [Tooltip("Speed at which the projectile or the creature will be pushed when attacking")]
    public float speed;

    [Header("NavMesh Checks")]
    public float navMeshSnapTolerance = 0.25f;
    public LayerMask groundLayers;

    // raycast settings to find ground under player
    public NavMeshPath _path;

    public bool TrySetDestination(Vector3 targetPosition)
    {
        // tries to snap the target onto the NavMesh
        if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, navMeshSnapTolerance, NavMesh.AllAreas))
            return false;

        // checks if it can even reach target position
        if (!navMeshAgent.CalculatePath(hit.position, _path))
            return false;

        if (_path.status != NavMeshPathStatus.PathComplete)
            return false;

        navMeshAgent.SetDestination(hit.position);
        return true;
    }

    public void DetectPlayer()
    {
        distanceToTarget = Vector3.Distance(navMeshAgent.transform.position, target.position);
        playerHeight = target.position.y;

        if (distanceToTarget < 15 && playerHeight < playerMaxHeight)
        {
            navMeshAgent.destination = target.position;
            targetFound = true;
        }
        else targetFound = false; return;
    }

    /// <summary>
    /// This allows the NPC/AI to wander around aimlessly on the navmesh
    /// </summary>

    public void SearchWalkPoint()
    {
        // Try a few times to find a valid point on the NavMesh within range
        const int maxAttempts = 12;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random point around the enemy
            Vector2 randomCircle = Random.insideUnitCircle * walkPointRange;
            Vector3 randomPoint = navMeshAgent.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Makes sure it is on the NavMesh
            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                continue;

            // Checks that it can reach it
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

    // TODO Implement Idle behavior when destination reached on Roam and Patrol

    // Sets a random walk point and moves to it
    public void Roam()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            navMeshAgent.SetDestination(walkPoint);
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && walkPointSet)
        {
            walkPointSet = false;
            currentState = AIState.Idle;
        }
        return;
    }

    // Sets waypoints for patrolling behavior
    public void Patrol()
    {
        if (wayPoints.Length == 0) return;
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f)
        {
            navMeshAgent.SetDestination(wayPoints[currentWaypoint]);
            currentWaypoint = (currentWaypoint + 1) % wayPoints.Length;
            return;
        }
    }

    // Idle state before resuming patrol
    // TODO add animations
    public IEnumerator Idle()
    {
        yield return new WaitForSeconds(idleTimer);
        Debug.Log("Idle State");
    }

    public IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(AttackInterval);
        alreadyAttacked = false;
        isAttacking = false;
    }

    public IEnumerator EnableNavmeshAgent()
    {
        yield return new WaitForSeconds(1);
        navMeshAgent.enabled = true;
    }

    public IEnumerator DelayAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void ProjectileAttack()
    {
        // Calculate direction to player
        Vector3 direction = (target.position - attackPt.position).normalized;
        // Add some random inaccuracy
        direction += new Vector3(Random.Range(-marginOfError, marginOfError), Random.Range(-marginOfError, marginOfError), Random.Range(-marginOfError, marginOfError));
        direction.Normalize();
        // Instantiate projectile and set its velocity
        GameObject projectile = Instantiate(attackObject, attackPt.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void ArcingProjectile()
    {
        // Calculate direction to player
        Vector3 toTarget = target.position - attackPt.position;
        Vector3 horizontal = new Vector3(toTarget.x, 0, toTarget.z); // Horizontal distance to target
        float horizontalDistance = horizontal.magnitude; // Magnitude of horizontal distance
        float verticalDistance = toTarget.y; // Vertical distance to target

        // Calculate the initial velocity required to hit the target
        float gravity = Physics.gravity.magnitude;
        float angle = Mathf.Deg2Rad * aimAngle; // Launch angle (45 degrees for optimal range)
        float speedSquared = (gravity * horizontalDistance * horizontalDistance) /
                             (2 * Mathf.Pow(Mathf.Cos(angle), 2) * (horizontalDistance * Mathf.Tan(angle) - verticalDistance));

        if (speedSquared <= 0)
        {
            Debug.LogWarning("Target is out of range for the given parameters.");
            return;
        }

        float speed = Mathf.Sqrt(speedSquared);

        // Calculate the velocity vector
        Vector3 velocity = horizontal.normalized * Mathf.Cos(angle) * speed;
        velocity.y = Mathf.Sin(angle) * speed;

        // Add some random inaccuracy
        velocity += new Vector3(Random.Range(-marginOfError, marginOfError), Random.Range(-marginOfError, marginOfError), Random.Range(-marginOfError, marginOfError));

        // Instantiate projectile and set its velocity
        GameObject projectile = Instantiate(attackObject, attackPt.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    public void ChargeAttack()
    {

        // Calculate direction to player
        Vector3 direction = (playerTransform.position - navMeshAgent.transform.position).normalized;
        // Add some random inaccuracy
        direction += new Vector3(Random.Range(-marginOfError, marginOfError), 0, Random.Range(-marginOfError, marginOfError));
        direction.Normalize();
        // Set velocity towards player
        navMeshAgent.velocity = direction * speed;

    }

    public void TakeDamage(int damage)
    {
        if (isDamagable == false) return;

        currentHealth -= damage;
        Debug.Log(name + " took damage: " + damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(name + " Died");
        Destroy(gameObject);
    }
}
