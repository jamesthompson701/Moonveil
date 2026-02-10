using JetBrains.Annotations;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    //temporary? reference to soil script
    private SoilObject soil;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

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
                    Debug.Log("Seed Planted");
                    Debug.Log("Seeds Remaining: " + PlayerInventory.instance.CheckSeeds());
                }

            }
            else
            {
                Debug.Log("Out of seeds");
            }

        }
        else
        {
            // Change to a random color
            rend.material.color = Random.ColorHSV();
        }

    }

    // Optional: A method to reset the color
    public void ResetColor()
    {
        rend.material.color = originalColor;
    }
}
