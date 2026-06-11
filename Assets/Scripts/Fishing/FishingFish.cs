using UnityEngine;

public class FishingFish : MonoBehaviour
{
  public FishData fishData;

    [Header("Element Sequence")]
    public ElementType[] elementSequence;

    [Header("Difficulty")]
    public float switchTime = 2f;
}
