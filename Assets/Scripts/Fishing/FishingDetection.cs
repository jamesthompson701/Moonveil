using System.Collections.Generic;
using UnityEngine;

public class FishingDetection : MonoBehaviour
{
    public List<FishingFish> fishInside = new List<FishingFish>();

    private void OnTriggerEnter(Collider other)
    {
        FishingFish fish = other.GetComponent<FishingFish>();

        if(fish != null && !fishInside.Contains(fish))
        {
            fishInside.Add(fish);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FishingFish fish = other.GetComponent<FishingFish>();

        if(fish != null)
        {
            fishInside.Remove(fish);
        }
    }
}