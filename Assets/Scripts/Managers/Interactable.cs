using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    // This method will be called by our ClickSelector
    public void OnInteract()
    {
        // Change to a random color
        rend.material.color = Random.ColorHSV();
    }

    // Optional: A method to reset the color
    public void ResetColor()
    {
        rend.material.color = originalColor;
    }
}
