using UnityEngine;
using TMPro;
//using UnityEngine.UI.Image;

[System.Serializable]
public class FishingBiomeUI : MonoBehaviour
{
    public FishingBiome biome;

    public Camera fishingCamera;

    public Canvas fishingCanvas;

    public TMP_Text promptText;

    public GameObject fishingVisuals;
}