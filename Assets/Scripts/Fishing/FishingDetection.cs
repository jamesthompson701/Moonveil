using UnityEngine;

public class FishingDetection : MonoBehaviour
{
 public FishingFish currentFish;

    private void OnTriggerEnter(Collider other)
    {
        FishingFish fish = other.GetComponent<FishingFish>();

        if (fish != null)
        {
            currentFish = fish;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FishingFish fish = other.GetComponent<FishingFish>();

        if (fish == currentFish)
        {
            currentFish = null;
        }
    }
}
