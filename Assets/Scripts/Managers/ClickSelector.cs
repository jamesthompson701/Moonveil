using UnityEngine;
using UnityEngine.InputSystem;

public class ClickSelector : MonoBehaviour
{
    private InputAction interactAction;

    public float raycastDistance = 100f;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += OnInteractReleased;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteractReleased;
        interactAction.Disable();
    }

    private void OnInteractReleased(InputAction.CallbackContext ctx)
    {
        // Only trigger on RELEASE
        if (ctx.phase != InputActionPhase.Performed) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.yellow);

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            Debug.Log(hit.collider);
            if (interactable != null)
            {
                interactable.OnInteract();
            }
        }
    }
}
