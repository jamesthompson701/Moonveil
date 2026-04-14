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

        manager.startFishingPrompt.text = "Press <i>Left Click<i> to start fishing";
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

        manager.startFishingPrompt.gameObject.SetActive(false);
    }
}
}