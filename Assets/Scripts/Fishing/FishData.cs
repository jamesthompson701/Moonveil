using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName = "Common Carp";

    // Random bite time after cast (seconds)
    public float biteDelayMin = 2f;
    public float biteDelayMax = 8f;

    // How long the player must keep the inner ring in bounds to succeed (seconds)
    public float keepDuration = 3f;

    // Ring growth speed parameters
    public float baseExpansionSpeed = 0.6f; // how fast inner ring grows automatically
    public float shrinkAmountPerPress = 0.15f; // how much pressing input shrinks the ring instantly
    public float maxInnerScale = 1.8f; // scale = failure (bubble pops)
    public float minInnerScale = 0.15f; // scale = failure (too small)
    
    // Optional probability or rarity
    public float catchWeight = 1f;
    public bool isShiny;
}