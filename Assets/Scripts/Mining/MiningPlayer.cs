using UnityEngine;
using UnityEngine.InputSystem;

public class MiningPlayer : MonoBehaviour
{
    InputAction interactAction;

    void start()
    {
        interactAction = InputSystem.actions.FindAction("Fire2");
    }
   void Update()
{
    if (MiningManager.Instance != null && MiningManager.Instance.isMining) return;

    if (interactAction.WasPressedThisFrame())
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, ~LayerMask.GetMask("IgnoreRaycast")))
        {
            MineRock rock = hit.collider.GetComponentInParent<MineRock>();

            if (rock != null)
            {
                rock.Interact();
            }
        }
    }
}
}
