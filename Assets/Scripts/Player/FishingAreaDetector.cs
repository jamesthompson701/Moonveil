using UnityEngine;

public class FishingAreaDetector : MonoBehaviour
{
public FishingManager manager;
private FishingArea current;

void Awake()
{
    if (manager == null)
    {
        manager = FindFirstObjectByType<FishingManager>();
    }
}
  void OnTriggerEnter(Collider other)
{
    FishingArea area = other.GetComponent<FishingArea>();
    if (area != null)
    {
        current = area;
        if (manager != null)
        {
            manager.SetCurrentArea(area);
            Debug.Log("Entered fishing area: " + area.name);
        }
        else
        {
            Debug.LogError("FishingManager not assigned on FishingAreaDetector!");
        }
    }
}

void OnTriggerExit(Collider other)
{
    FishingArea area = other.GetComponent<FishingArea>();
    if (area != null && area == current)
    {
        current = null;
        manager.ClearCurrentArea(area);
    }
}
}