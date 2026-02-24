using UnityEngine;

public class SpellDamageManager : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float force;
    private int attackChoice;

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
            Destroy(gameObject);
            return;
        }

        MeleeEnemyManager melee = other.GetComponent<MeleeEnemyManager>();
        ChargingEnemyManager charge = other.GetComponent<ChargingEnemyManager>();
        StraightShotEnemyManager straight = other.GetComponent<StraightShotEnemyManager>();
        ArcShotEnemyManager arc = other.GetComponent<ArcShotEnemyManager>();

        if (attackChoice == 1)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

            foreach (Collider hit in hits)
            {
                ApplyDamageAndForce(hit);
            }
        }
        else if (attackChoice == 2)
        {
            ApplyDamage(other);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
            }
        }
        else if (attackChoice == 3)
        {
            ApplyDamage(other);
        }
        else if (attackChoice == 4)
        {
            ApplyDamage(other);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private void ApplyDamage(Collider target)
    {
        CreatureDefs creature = target.GetComponentInParent<CreatureDefs>();
        if (creature != null)
        {
            creature.TakeDamage(damage);
            return;
        }

        MeleeEnemyManager melee = target.GetComponentInParent<MeleeEnemyManager>();
        if (melee != null) melee.TakeDamage(damage);

        ChargingEnemyManager charge = target.GetComponentInParent<ChargingEnemyManager>();
        if (charge != null) charge.TakeDamage(damage);

        StraightShotEnemyManager straight = target.GetComponentInParent<StraightShotEnemyManager>();
        if (straight != null) straight.TakeDamage(damage);

        ArcShotEnemyManager arc = target.GetComponentInParent<ArcShotEnemyManager>();
        if (arc != null) arc.TakeDamage(damage);
    }

    private void ApplyDamageAndForce(Collider target)
    {
        ApplyDamage(target);

        PlayerManager player = target.GetComponent<PlayerManager>();
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (player.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * force, ForceMode.Impulse);
            }
        }
    }
}
