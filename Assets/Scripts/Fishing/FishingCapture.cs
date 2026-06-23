using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class FishingCapture : MonoBehaviour
{
    public FishingDetection detection;

    public Transform bubbleAnchor;

    void Update()
    {
        Debug.Log("Capture update");

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

            FishMovement movement = fish.GetComponent<FishMovement>();

            if(movement != null)
            {
                movement.enabled = false;
            }
        }

        if(capturedFish.Count <= 0)
        {
            return;
        }

        Debug.Log("Space pressed");
        Debug.Log("Fish Count: " + capturedFish.Count);

        FishingManager.Instance.StartBubblePhase(capturedFish);
    }
}