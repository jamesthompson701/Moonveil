using UnityEngine;

public class ClickSelector : MonoBehaviour
{

    public float raycastDistance = 100f; // How far the ray will travel

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.yellow);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }
    }
}
