using UnityEngine;

/// <summary>
/// Handles projectile spell collision and applies effects based on spell type.
/// </summary>
public class SpellDamageManager : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float force;
    [SerializeField] private float _radius;
    private int attackChoice;
    private ProjectileSpells.SpellType spellType;
    private GameObject caster;

    /// <summary>
    /// Initialize projectile spell with type and caster.
    /// </summary>
    public void InitProjectile(int choice, int dmg, ProjectileSpells.SpellType type, GameObject casterObj)
    {
        attackChoice = choice;
        damage = dmg;
        spellType = type;
        caster = casterObj;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            if (spellType == ProjectileSpells.SpellType.Fire)
            {
                // Fire: AOE burn
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    ApplyDamage(hit);
                    ApplyBurn(hit); // <-- Connect Burn
                    ApplyKnockback(hit, 5f); // Small knockback
                }
            }
            else if (spellType == ProjectileSpells.SpellType.Water)
            {
                // Water: slip zone
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    ApplySlip(hit); // <-- Connect Slip
                }
            }
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Enemy")) return;

        switch (spellType)
        {
            case ProjectileSpells.SpellType.Fire:
                // Fire: AOE burn
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    ApplyDamage(hit);
                    ApplyBurn(hit); // <-- Connect Burn
                    ApplyKnockback(hit, 5f); // Small knockback
                }
                break;
            case ProjectileSpells.SpellType.Water:
                // Water: slip zone
                Collider[] hits2 = Physics.OverlapSphere(transform.position, _radius);
                ApplyDamage(other);
                foreach (Collider hit in hits2)
                {
                    ApplySlip(hit); // <-- Connect Slip
                }
                break;
            case ProjectileSpells.SpellType.Air:
                ApplyDamage(other);
                ApplyKnockback(other, 15f); // Strong knockback
                break;
            default:
                ApplyDamage(other);
                break;
        }

        Destroy(gameObject);
    }

    private void ApplyDamage(Collider target)
    {
        CreatureDefs creature = target.GetComponentInParent<CreatureDefs>();
        if (creature != null)
        {
            creature.TakeDamage(damage, target.ClosestPoint(transform.position), (target.transform.position - transform.position).normalized, force, caster);
        }
    }

    private void ApplyKnockback(Collider target, float knockbackForce)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }

    // --- ADDED: ApplyBurn using EnemyStatusReceiver ---
    private void ApplyBurn(Collider target)
    {
        // Only apply to enemies
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();

        // Example values: 3 seconds, 5 DPS, caster's transform
        status.ApplyBurn(3f, 5f, caster != null ? caster.transform : null);
    }

    private void ApplySlip(Collider target)
    {
        // Only apply to enemies
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();

        // Example values: 2 seconds, 0.2 damping multiplier (very slippery)
        //status.ApplySlip(2f, 0.2f);
    }
}
