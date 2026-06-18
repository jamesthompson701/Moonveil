using System.Collections.Generic;
using UnityEngine;

public class FishingCapture : MonoBehaviour
{
    public FishingDetection detection;

    public Transform bubbleAnchor;

    void Update()
    {
        if (!FishingManager.Instance.inFishingMode)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureFish();
        }
    }

    void CaptureFish()
    {
        List<FishingFish> capturedFish = new List<FishingFish>();

        foreach(FishingFish fish in detection.fishInside)
        {
            capturedFish.Add(fish);

            fish.transform.SetParent(bubbleAnchor);

            fish.transform.localPosition = Random.insideUnitSphere * 0.5f;
        }

        if(capturedFish.Count <= 0)
        {
            return;
        }

        FishingManager.Instance.StartBubblePhase(capturedFish);
    }    
}