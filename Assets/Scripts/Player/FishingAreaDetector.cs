using UnityEngine;

public class FishingAreaDetector : MonoBehaviour
{
    public FishingManager manager;
    private FishingArea current;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger with: " + other.name);
        FishingArea area = other.GetComponent<FishingArea>();
        if (area != null)
        {
            current = area;
            manager.SetCurrentArea(area);
            Debug.Log("Entered fishing area");
            manager.startFishingPrompt.text = "Press " + manager.startFishingInput + " to start fishing";
            manager.startFishingPrompt.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        FishingArea area = other.GetComponent<FishingArea>();
        if (area != null && area == current)
        {
            current = null;
            manager.ClearCurrentArea(area);
            Debug.Log("Left fishing area");
            manager.startFishingPrompt.gameObject.SetActive(false);
        }
    }
}