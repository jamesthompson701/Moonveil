using System.Collections;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public GameObject baitPrefab;
    public Transform castOrigin; // where bait should appear from (rod tip)
    public float castDelay = 1f;

    FishingManager manager;
    string reelInputName;
    private bool isCasted = false;
    private GameObject spawnedBait;

    public void Initialize(FishingManager mgr, string reelInput)
    {
        manager = mgr;
        reelInputName = reelInput;
        isCasted = false;
    }

    void Update()
    {
        if (manager == null) return;

        // Cast with manager's castInput (use GetButtonDown for simplicity)
        if (!isCasted && Input.GetButtonDown(manager.castInput))
        {
            isCasted = true; // lock immediately to prevent double cast
            Vector3 targetPos = GetWaterHitPoint();
            StartCoroutine(CastRodCoroutine(targetPos));
        }

        // Pull rod (cancel) - right mouse or manager.reelInput used earlier to begin reeling; use right click to pull
        if (isCasted && Input.GetMouseButtonDown(1))
        {
            PullRod();
        }
    }

    Vector3 GetWaterHitPoint()
    {
        // raycast from fishing camera center to find water plane.
        Camera cam = manager.fishingCamera;
        if (cam == null) cam = manager.mainCamera;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            return hit.point + Vector3.up * 0.05f; // slight offset
        }
        // fallback: a point in front of camera
        return cam.transform.position + cam.transform.forward * 10f;
    }

    IEnumerator CastRodCoroutine(Vector3 targetPosition)
    {
        // play a cast animation here once we have it
        yield return new WaitForSeconds(castDelay);

        spawnedBait = Instantiate(baitPrefab);
        spawnedBait.transform.position = targetPosition;
        Debug.Log("Rod casted. Bait in the water.");

        // notify manager
        manager.OnRodCasted(targetPosition);
    }

    public void PullRod()
    {
        if (!isCasted) return;
        isCasted = false;
        if (spawnedBait) Destroy(spawnedBait);
        manager.OnRodPulled();
    }

    // called when a fish was caught and you want to update visuals (pull rod, remove bait)
    public void OnCaughtFish()
    {
        if (spawnedBait) Destroy(spawnedBait);
        isCasted = false;
        // add animations reel, reward, etc.
    }
}