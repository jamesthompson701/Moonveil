using UnityEngine;

public class FishingAreaDetector : MonoBehaviour
{
    public FishingManager manager;
    private FishingArea current;

    void OnTriggerEnter(Collider other)
    {
        FishingArea area = other.GetComponent<FishingArea>();
        if (area != null)
        {
            current = area;
            manager.SetCurrentArea(area);
            Debug.Log("Entered fishing area");
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
        }
    }
}