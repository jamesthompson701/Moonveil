using UnityEngine;

public class TreeFinder : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; // Select your layer in the Inspector
    [SerializeField] private float maxDistance = 50f;

    void Update()
    {
    // Cast a ray forward from the object
    Ray ray = new Ray(transform.position, transform.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, targetLayer))
    {
        Debug.Log($"Hit an object on the correct layer: {hit.collider.name}");
    }
}
}
