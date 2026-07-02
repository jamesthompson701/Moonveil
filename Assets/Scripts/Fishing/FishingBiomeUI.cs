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

    // element sprite swapping
    // the UI image component
    /*public Image targetUiImage; 

    // new sprite you want to display
    public Sprite newSprite; 

    // Call this function via a UI Button click or another game event
    public void SwapSprite()
    {
        if (targetUiImage != null && newSprite != null)
        {
            // This line performs the actual swap
            targetUiImage.sprite = newSprite; 
        }
    }*/
}