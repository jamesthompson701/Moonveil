using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    public static bool HasHighlightedInteractable { get; private set; }

    public GameObject leftClickIcon;

    // Currently highlighted object from the interaction raycast
    private GameObject currentHighlighted;

    private void OnDisable()
    {
        ClearHighlight();
        HasHighlightedInteractable = false;
    }

    private void Update()
    {
        // Use the same raycast as ClickSelector: cast from the main camera through the mouse.
        if (Camera.main == null || ClickSelector.Instance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool foundInteractable = false;
        if (Physics.Raycast(ray, out hit, ClickSelector.Instance.raycastDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
            }

            if (interactable != null)
            {
                GameObject toHighlight = interactable.gameObject;
                if (toHighlight != currentHighlighted)
                {
                    ClearHighlight();
                    ApplyHighlight(toHighlight);
                }
                foundInteractable = true;
            }
        }

        HasHighlightedInteractable = foundInteractable;

        if (!foundInteractable)
        {
            ClearHighlight();
        }
    }

    private void ApplyHighlight(GameObject target)
    {
        if (target == null) return;

        // Add Outline if not present
        var outline = target.GetComponent<Outline>();
        if (outline == null)
        {
            outline = target.AddComponent<Outline>();
        }

        // Style based on tag (matches previous behavior)
        if (target.CompareTag("Dialogue"))
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 5f;
        }
        else
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
        }

        if (leftClickIcon != null)
        {
            leftClickIcon.SetActive(true);
        }

        currentHighlighted = target;
    }

    private void ClearHighlight()
    {
        if (currentHighlighted != null)
        {
            var existing = currentHighlighted.GetComponent<Outline>();
            if (existing != null)
            {
                Destroy(existing);
            }
            currentHighlighted = null;
        }

        if (leftClickIcon != null)
        {
            leftClickIcon.SetActive(false);
        }
    }
}
