using UnityEngine;
using UnityEngine.InputSystem;

public class ClickSelector : MonoBehaviour
{
    private InputAction interactAction;

    public float raycastDistance = 100f;

    //Singleton
    public static ClickSelector Instance;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

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
            
            //If its a fishin area tells the manager to change the fishing area
            FishingArea currentArea;
            if (currentArea = hit.collider.GetComponent<FishingArea>())
            {
                Debug.Log("Fishing Area set to" + currentArea);
                FishingManager.Instance.currentArea = currentArea;
            }

            Debug.Log("Interacted with " + hit.collider);
            if (interactable != null)
            {
                interactable.OnInteract();
            }
        }

        
    }
}
