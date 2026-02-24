using UnityEngine;
using System.Collections.Generic;

public enum FishingBiome
{Water, Fire, Ice}
[RequireComponent(typeof(Collider))]
public class FishingArea : MonoBehaviour
{
    [Tooltip("Camera to use while fishing (disable by default)")]
    public Camera fishingCamera;

    [Tooltip("Fish that can be caught in this area.")]
    public List<FishData> fishInThisArea = new List<FishData>();

    [Tooltip("Optional display name for the area")]
    public string areaName = "Lake";

    public FishingBiome biome;

    // Called by fishingManager to request a fish from this area
    public FishData GetRandomFish()
    {
        if (fishInThisArea == null || fishInThisArea.Count == 0) return null;
        // random selection
        int idx = Random.Range(0, fishInThisArea.Count);
        return fishInThisArea[idx];
    }
}