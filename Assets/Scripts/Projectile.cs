using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject slipIce;
    [SerializeField] private float slipDuration = 3.0f;

    private bool spawnedSlip = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore trigger volumes like "CombatArea", detection zones, etc.
        if (other.isTrigger) return;

        // ignore player so you don't delete it instantly on spawn
        if (other.CompareTag("Player")) return;

        // Only react to ground (and only for water)
        if (!spawnedSlip && CompareTag("WaterAttack") && other.CompareTag("Ground"))
        {
            spawnedSlip = true;

            if (slipIce == null)
            {
                Debug.LogWarning($"{nameof(Projectile)} on {name} has no slipIce assigned.");
                Destroy(gameObject);
                return;
            }

            //Spawn at closest point on collider instead of transform.position
            Vector3 spawnPos = transform.position;

            GameObject clone = Instantiate(slipIce, spawnPos, Quaternion.identity);
            Destroy(clone, slipDuration);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        //if (other.CompareTag("Enemy")) Destroy(gameObject);
    }
}
