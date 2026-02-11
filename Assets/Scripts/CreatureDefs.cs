using StarterAssets;
using System.Collections;
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
    public float attackDamage;
    public float maxHealth;
    public float currentHealth;
    public float idleTimer;
    public float distanceToTarget;
    public Vector3[] wayPoints;
    public Vector3 _resetPosition;
    public bool isRoamingAI;
    public bool _hasResetPosition;
    public bool _wasPlayerInSightRange;
    public float playerHeight;
    public ThirdPersonController playerController;


    [Header("Patroling")]
    public Vector3 walkPoint;
    public bool walkPointSet;
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

    [Header("Projectile Attack")]
    public float marginOfError;
    public float projectileSpeed;

    [Header("NavMesh Checks")]
    public float navMeshSnapTolerance = 0.25f;
    public Vector3 navCheckPoint;
    public float _airborneGraceTimer = 0f;
    public float airborneGraceTime = 0.5f; // seconds to allow "airborne" player to still be reachable
    public LayerMask groundLayers;

    // raycast settings to find ground under player
    public float playerGroundRayDistance = 6f;
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

        // Check NavMesh under the player instead of at player's current (possibly airborne) position
        Vector3 navCheckPoint = target.position;

        if (distanceToTarget < 15 && playerHeight < 2)
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
}
