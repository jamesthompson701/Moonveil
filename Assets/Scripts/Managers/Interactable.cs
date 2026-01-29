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
            soil.SpawnCrop();
            PlayerInventory.instance.AddSeeds(-1);

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
