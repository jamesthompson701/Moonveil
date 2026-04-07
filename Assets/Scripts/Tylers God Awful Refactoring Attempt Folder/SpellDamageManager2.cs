using System.Collections;
using UnityEngine;

/// <summary>
/// Manager for handling spell damage and effects. 
/// This class will be responsible for applying damage and status effects to enemies hit by spells, as well as managing spell interactions and cooldowns. 
/// It will work in conjunction with the individual spell classes (GroundTargetSpells, ProjectileSpells2, StreamSpells) 
/// to ensure that the correct damage and effects are applied based on the spell type and enemy resistances.
/// </summary>
public class SpellDamageManager2 : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float force;
    [SerializeField] private float _radius;
    private int attackChoice;
    private SO_SpellDefs2.SpellType spellType;
    private GameObject caster;
    [SerializeField] private bool isProjectile = false;

    public void InitProjectile2(int choice, int dmg, ProjectileSpells2.SpellType type, GameObject casterObj)
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
            if (spellType == ProjectileSpells2.SpellType.Fire)
            {
                // Apply direct hit effects to the ground
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    ApplyDamage(hit);
                    ApplyBurn(hit);
                    ApplyKnockback(hit, force);
                }
            }
            else if (spellType == ProjectileSpells2.SpellType.Water)
            {
                StartCoroutine(ApplyWaterEffect());
            }
            if (isProjectile)
                Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Enemy")) return;

        switch (spellType)
        {
            case ProjectileSpells2.SpellType.Fire:
                // Always apply direct hit effects
                ApplyDamage(other);
                ApplyBurn(other);
                ApplyKnockback(other, force);

                // Then apply area effects
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    if (hit != other) // Avoid double-applying to the same enemy
                    {
                        ApplyDamage(hit);
                        ApplyBurn(hit);
                        ApplyKnockback(hit, force);
                    }
                }
                break;
            case ProjectileSpells2.SpellType.Water:
                // Always apply direct hit effects
                ApplyDamage(other);
                ApplySlow(other);

                // Then apply area effects via coroutine
                StartCoroutine(ApplyWaterEffect());
                break;
            case ProjectileSpells2.SpellType.Air:
                ApplyDamage(other);
                ApplyKnockback(other, force);
                break;
            case ProjectileSpells2.SpellType.Earth:
                ApplyDamage(other);
                ApplyRoot(other);
                break;
            default:
                ApplyDamage(other);
                break;
        }
        if (isProjectile)
            Destroy(gameObject);
    }

    private IEnumerator ApplyWaterEffect()
    {
        float duration = 5f; // Example duration for the water effect
        float interval = 1f; // Damage and slow application interval
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    ApplyDamage(hit);
                    ApplySlow(hit);
                }
            }
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
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
            Vector3 dir = (target.transform.position - transform.position);
            dir.y = 0f; // Prevent vertical knockback
            dir = dir.normalized;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }

    private void ApplyBurn(Collider target)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplyBurn(3f, 5f, caster != null ? caster.transform : null);
    }

    private void ApplySlow(Collider target)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplySlow(2f, 0.5f); // Example values: 2 seconds, 50% speed reduction
    }

    private void ApplyRoot(Collider target)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplyRoot(2f);
    }
}
