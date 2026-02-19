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
            //reference to soil being clicked
            soil = gameObject.GetComponent<SoilObject>();
            if (PlayerInventory.instance.CheckSeeds() > 0)
            {
                //make sure soil is empty and tilled before removing a seed and spawning the crop
                if (soil.GetComponent<SoilObject>().soilContent == SoilContent.empty && soil.tilled)
                {
                    soil.SetPlantType(PlayerInventory.instance.seedRef);
                    soil.SpawnCrop();
                    PlayerInventory.instance.AddSeeds(-1, PlayerInventory.instance.seedRef);
                    Debug.Log("Seed Planted");
                   // Debug.Log("Seeds Remaining: " + PlayerInventory.instance.CheckSeeds());
                }
                else
                {
                    Debug.Log("Not ready for planting, Tilled status: " + soil.tilled + ", Soil Content: " + soil.GetComponent<SoilObject>().soilContent);
                }

            }
            else
            {
                Debug.Log("Out of seeds");
            }

        }
        else if(gameObject.CompareTag("fishingArea"))
        {
            FishingManager.Instance.EnterFishingMode(FishingManager.Instance.currentArea);
        }
        else
        {
            Debug.Log("Nothing interactable hit, tag is: " + gameObject.tag);
        }

    }

    // Optional: A method to reset the color

}
