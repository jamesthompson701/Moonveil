using UnityEngine;
using System.Collections;

public class FishingFish : MonoBehaviour
{
  public Vector3 startPosition;
  public FishData fishData;

  [Header("Difficulty")]
  public float switchTime = 2f;

  void Start()
  {
    startPosition = transform.position;
  }


  public void ResetFish()
  {
    transform.SetParent(null);

    transform.position = startPosition;

    UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    FishMovement movement = GetComponent<FishMovement>();

    if(agent != null)
    {
      agent.Warp(startPosition);
      movement.enabled = true;
    }

    gameObject.SetActive(true);
  }

  public void RemoveFish(float respawnTime)
  {
    transform.SetParent(null);
    
    StartCoroutine(RespawnRoutine(respawnTime));

    FishMovement movement = GetComponent<FishMovement>();

    if(movement != null)
    {
      movement.enabled = true;
    }
  }

  IEnumerator RespawnRoutine(float time)
  {
    gameObject.SetActive(false);

    yield return new WaitForSeconds(time);

    transform.position = startPosition;

    UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    if(agent != null)
    {
      agent.Warp(startPosition);
    }

    gameObject.SetActive(true);
  }
}
