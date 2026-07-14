using UnityEngine;
using System.Collections.Generic;

public enum FishingBiome
{Water, Fire, Ice}
[RequireComponent(typeof(Collider))]
public class FishingArea : MonoBehaviour
{

[Header("New Fishing")]

public Transform fishContainer;

public float catchRadius = 5f;


[Tooltip("Camera to use while fishing (disable by default)")]
public Camera fishingCamera;

[Tooltip("Fish that can be caught in this area.")]
public List<FishData> fishInThisArea = new List<FishData>();

[Tooltip("Optional display name for the area")]
public string areaName = "Lake";

public FishingBiome biome;

// called by fishingManager to request a fish from this area
    public FishData GetRandomFish()
    {
        if (fishInThisArea == null || fishInThisArea.Count == 0) 
        {
            return null;
        }

        // random selection
        int idx = Random.Range(0, fishInThisArea.Count);
        return fishInThisArea[idx];
    }

    // water spell to enter fishing
    private void OnTriggerEnter(Collider other)
    {
        //testing for why the dock itself can enter fishing
        Debug.Log("Water Spell Can Hit");
        if (other.CompareTag("WateringSpell"))
        {
            Debug.Log("Water Spell Hit");

            if (!HasAvailableFish())
            {
                FishingManager.Instance.ShowFishingPrompt("No fish are available right now.");
                return;
            }

            FishingManager.Instance.EnterFishingMode(this);
        }
    }

    public bool HasAvailableFish()
    {
        if (fishContainer == null)
        {
            return false;
        }

        FishingFish[] fish = fishContainer.GetComponentsInChildren<FishingFish>(true);

        foreach (FishingFish availableFish in fish)
        {
            if (availableFish.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}