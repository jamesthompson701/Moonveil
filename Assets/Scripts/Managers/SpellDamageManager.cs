using UnityEngine;

public class SpellDamageManager : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float force;
    private int attackChoice;
    [SerializeField] private float _radius;

    public void Init(int choice, int dmg, float f)
    {
        attackChoice = choice;
        damage = dmg;
        force = f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            if (attackChoice == 1) // Fire Attack
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                Debug.Log(("Fire Attack hit " + hits.Length + " targets."));
                foreach (Collider hit in hits)
                {
                    ApplyDamage(hit);
                }
            }
            Destroy(gameObject);
            return;
        }

        // Fire Attack
        if (attackChoice == 1)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            Debug.Log(("Fire Attack hit " + hits.Length + " targets."));

            foreach (Collider hit in hits)
            {
                ApplyDamage(hit);
            }
        }
        // Earth Attack
        else if (attackChoice == 2)
        {
            ApplyDamage(other);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }
        // Water Attack
        else if (attackChoice == 3)
        {
            ApplyDamage(other);
        }
        // Air Attack
        else if (attackChoice == 4)
        {
            ApplyDamage(other);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce((Vector3.up + dir).normalized * force, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private void ApplyDamage(Collider target)
    {
        CreatureDefs creature = target.GetComponentInParent<CreatureDefs>();
        if (creature != null)
        {
            creature.TakeDamage(damage, target.ClosestPoint(transform.position), (target.transform.position - transform.position).normalized, force, gameObject);
        }
    }

    private void ApplyDamageAndForce(Collider target)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

        foreach (Collider hit in hits)
        {
            ApplyDamage(hit);

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }
    }
}
