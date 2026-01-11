using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject slipIce;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore trigger volumes like "CombatArea", detection zones, etc.
        if (other.isTrigger) return;

        // ignore player so you don't delete it instantly on spawn
        if (other.CompareTag("Player")) return;

        // Only react to ground (and only for water)
        if (CompareTag("WaterAttack") && other.CompareTag("Ground"))
        {
            GameObject clone = Instantiate(slipIce, transform.position, Quaternion.identity);
            Destroy(clone, 3.0f);

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
