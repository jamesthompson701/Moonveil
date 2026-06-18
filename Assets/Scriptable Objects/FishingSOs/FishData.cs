using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName = "Common Carp";

    // Optional probability or rarity
    public float catchWeight = 1f;
    public bool isShiny;

    // ItemSO
    public ItemSO fishItem;
}