using UnityEngine;
using System.Collections;

public class FishingFish : MonoBehaviour
{
  public Vector3 startPosition;
  public FishData fishData;

    [Header("Element Sequence")]
    public ElementType[] elementSequence;

    [Header("Difficulty")]
    public float switchTime = 2f;

    void Start()
  {
      startPosition = transform.position;
  }

  public void ResetFish()
  {
    transform.position = startPosition;
    gameObject.SetActive(true);
  }

  public void RemoveFish(float respawnTime)
  {
    StartCoroutine(RespawnRoutine(respawnTime));
  }

  IEnumerator RespawnRoutine(float time)
  {
    gameObject.SetActive(false);

    yield return new WaitForSeconds(time);

    transform.position = startPosition;
    gameObject.SetActive(true);
  }
}
