using UnityEngine;

public class MiningPlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Fire2
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

        if (Input.GetMouseButtonDown(1))
    {
        Debug.Log("Right click detected");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, ~LayerMask.GetMask("IgnoreRaycast")))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            MineRock rock = hit.collider.GetComponentInParent<MineRock>();

            if (rock != null)
            {
                Debug.Log("Rock found, interacting");
                rock.Interact();
            }
            else
            {
                Debug.Log("Hit something, but no MineRock");
            }
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }
    }
}
