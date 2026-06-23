using UnityEngine;

public class BubbleVisabilityCheck : MonoBehaviour
{
    bool bubbleInside;
    float timer;

    void Update()
    {
        if(FishingManager.Instance.currentPhase != FishingManager.FishingPhase.Bubble)
        {
            timer = 0f;
            return;
        }
        
        if(bubbleInside)
        {
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;

            if(timer >= 3f)
            {
                FishingManager.Instance.FailFishing();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<FishingBubble>())
        {
            bubbleInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<FishingBubble>())
        {
            bubbleInside = false;
        }
    }
}