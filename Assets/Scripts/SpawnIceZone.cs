using UnityEngine;

public class SpawnIceZone : MonoBehaviour
{
    [SerializeField] private GameObject slipIce;
    [SerializeField] private float slipDuration = 3.0f;
    [SerializeField] private float force = 5f;

    private bool spawnedSlip = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore trigger volumes like "CombatArea", detection zones, etc.
        if (other.isTrigger) return;

        // ignore player so you don't delete it instantly on spawn
        if (other.CompareTag("Player")) return;

        // Only react to ground (and only for water)
        if (!spawnedSlip && (other.CompareTag("Ground") || other.CompareTag("Enemy")))
        {
            spawnedSlip = true;

            if (slipIce == null)
            {
                Debug.LogWarning($"{nameof(SpawnIceZone)} on {name} has no slipIce assigned.");
                Destroy(gameObject);
                return;
            }

            //Spawn at closest point on collider instead of transform.position
            Vector3 spawnPos = transform.position;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 slipDir = rb.linearVelocity.normalized;
                rb.AddForce(slipDir * force, ForceMode.Impulse);
            }

            GameObject clone = Instantiate(slipIce, spawnPos, Quaternion.identity);
            Destroy(clone, slipDuration);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground") || other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
