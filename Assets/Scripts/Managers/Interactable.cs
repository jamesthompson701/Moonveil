using JetBrains.Annotations;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    //temporary? reference to soil script
    private SoilObject soil;



    // This method will be called by our ClickSelector
    public void OnInteract()
    {
        if (gameObject.CompareTag("Soil"))
        {
            soil = gameObject.GetComponent<SoilObject>();
            if (PlayerInventory.instance.CheckSeeds() > 0)
            {
                if (soil.GetComponent<SoilObject>().soilContent == SoilContent.empty)
                {
                    soil.SpawnCrop();
                    PlayerInventory.instance.AddSeeds(-1);
                    PlayerInventory.instance.UpdateSeeds();
                    Debug.Log("Seed Planted");
                    Debug.Log("Seeds Remaining: " + PlayerInventory.instance.CheckSeeds());
                }

            }
            else
            {
                Debug.Log("Out of seeds");
            }

        }
        if(gameObject.CompareTag("fishingArea"))
        {
            FishingManager.Instance.EnterFishingMode(FishingManager.Instance.currentArea);
        }
        else
        {
            Debug.Log("Nothing interactable hit");
        }

    }

    // Optional: A method to reset the color

}
