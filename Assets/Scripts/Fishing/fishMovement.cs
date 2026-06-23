using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class FishMovement : MonoBehaviour
{

    public NavMeshAgent agent;

    public float range; //radius of sphere



    public Transform centrePoint; //centre of the area the agent wants to move around in

    //public bool isFocusedFish;

    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh) 
        {
            
            Debug.Log("NavMeshAgent not on NavMesh");
            return;
        }
    }


    void Update()
    {
        /*if (isFocusedFish)
        {
            Debug.Log("agent " + agent + "enabled " + agent.enabled + "nm: " + agent.isOnNavMesh);
        }*/
        
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;

            if (RandomPoint(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);

                agent.SetDestination(point);

                //Debug.Log("point: " + point);
            }
        }
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {



        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range; //random point in a sphere 

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to change

            result = hit.position;

            return true;
        }

            result = Vector3.zero;

            return false;
    }

    void OnEnable()
    {
        Debug.Log(name + " enabled");
    }
}