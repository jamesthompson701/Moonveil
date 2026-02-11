using UnityEngine;
using UnityEngine.AI;

public class SO_MeleeEnemy : CreatureDefs
{
    private void Awake()
    {
        currentState = AIState.Patrol;
        player= GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<SpellManager>().hitPt;
        navMeshAgent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        playerTransform = player.transform;
    }

    void Update()
    {
        if (navMeshAgent.enabled)
        {
            DetectPlayer();
        }

        switch (currentState)
        {
            case AIState.Idle:

                StartCoroutine(Idle());
                currentState = AIState.Patrol;

                break;

            case AIState.Patrol:
                if (isRoamingAI)
                {
                    Roam();
                    if (targetFound)
                    {
                        // store the position we were at when we first spotted the player
                        if (!_hasResetPosition)
                        {
                            _resetPosition = navMeshAgent.transform.position;
                            _hasResetPosition = true;
                        }
                        currentState = AIState.Combat;
                    }
                }
                else
                {
                    Patrol();
                    if (targetFound)
                    {
                        // store last waypoint position we were at when we first spotted the player
                        if (!_hasResetPosition)
                        {
                            _resetPosition = wayPoints[currentWaypoint];
                            _hasResetPosition = true;
                        }
                        currentState = AIState.Combat;
                    }
                }
                    break;

            case AIState.Combat:
                // Immediate reset if player is above height threshold and grounded.
                if (playerController != null && playerHeight > 2 && playerController.Grounded)
                {
                    currentState = AIState.Reset;
                    break;
                }

                if (navMeshAgent.enabled == true)
                {
                    transform.LookAt(playerTransform);
                    navMeshAgent.SetDestination(playerTransform.position);  
                }

                distanceToTarget = Vector3.Distance(navMeshAgent.transform.position, playerTransform.position);

                if (distanceToTarget <= attackRange && !isAttacking)
                {
                    // Attack the player
                    if (!alreadyAttacked)
                    {
                        isAttacking = true;
                        navMeshAgent.enabled = false;
                        GameObject clone = Instantiate(attackObject, attackPt.position, Quaternion.identity);
                        Destroy(clone, 2f);
                        // Melee attack logic here
                        Debug.Log("Melee Attack!");
                        // Set attack cooldown
                        alreadyAttacked = true;
                        StartCoroutine(attackCooldown());
                        StartCoroutine(EnableNavmeshAgent());
                    }
                }
                else if (distanceToTarget > 30 || (playerController != null && playerHeight > 2 && playerController.Grounded))
                {
                    currentState = AIState.Reset;
                }

                break;

            case AIState.Reset:

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
                        }
                    }
                    else
                    {
                        // If we can't find navmesh near home, just patrol
                        _hasResetPosition = false; 
                    }
                }
                else
                {
                    // Fallback behavior (no stored reset point)
                    navMeshAgent.ResetPath();                 
                }

                currentState = AIState.Idle;
                Debug.Log("Run Away!");
                break;
        }
    }
}
