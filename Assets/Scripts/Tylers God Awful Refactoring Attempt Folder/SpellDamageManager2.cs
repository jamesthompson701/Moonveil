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
    [SerializeField] private float iceDamagePerSecond;
    [SerializeField] private float aoeDamage;
    [SerializeField] private float force;
    [SerializeField] private float _radius;
    [SerializeField] private float duration;
    private SO_SpellDefs2.SpellType spellType;
    [SerializeField] private bool isProjectile = false;
    [SerializeField] private bool isBasicAttack = false;

    //TODO Change from spellType to unique bools to be set per prefab to determine the type of effect applied

    public void InitProjectile2(int dmg, SO_SpellDefs2.SpellType type)
    {
        damage = dmg;
        spellType = type;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            if (spellType == SO_SpellDefs2.SpellType.Fire)
            {
                // Apply direct hit effects to the ground
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    ApplyDamage(hit, aoeDamage);
                    ApplyBurn(hit);
                    ApplyKnockback(hit, force);
                }
            }
            else if (spellType == SO_SpellDefs2.SpellType.Water)
            {
                StartCoroutine(ApplyWaterEffect(duration));
            }
            if (isProjectile)
                Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Enemy")) return;

        switch (spellType)
        {
            case SO_SpellDefs2.SpellType.Fire:
                // Always apply direct hit effects
                ApplyDamage(other, damage);
                ApplyBurn(other);
                ApplyKnockback(other, force);

                // Then apply area effects
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    if (hit != other) // Avoid double-applying to the same enemy
                    {
                        ApplyDamage(hit, aoeDamage);
                        ApplyBurn(hit);
                        ApplyKnockback(hit, force);
                    }
                }
                break;
            case SO_SpellDefs2.SpellType.Water:
                // Always apply direct hit effects
                ApplyDamage(other, damage);
                ApplySlow(other, duration, 0.5f);
                // Then apply area effects via coroutine
                StartCoroutine(ApplyWaterEffect(duration)); // Use duration
                break;
            case SO_SpellDefs2.SpellType.Air:
                ApplyDamage(other, damage);
                ApplyKnockback(other, force);
                break;
            case SO_SpellDefs2.SpellType.Earth:
                ApplyDamage(other, damage);
                ApplyRoot(other);
                break;
            default:
                ApplyDamage(other, damage);
                break;
        }
        if (isBasicAttack)
            ApplyDamage(other, damage);

        if (isProjectile)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (spellType == SO_SpellDefs2.SpellType.Water)
        {
            ApplyDamage(other, iceDamagePerSecond);
        }
    }

    private IEnumerator ApplyWaterEffect(float duration)
    {
        float interval = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    ApplySlow(hit, duration, 0.5f); // Use duration
                }
            }
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private void ApplyDamage(Collider target, float damage)
    {
        CreatureDefs creature = target.GetComponentInParent<CreatureDefs>();
        if (creature != null)
        {
            creature.TakeDamage(damage, null);
        }
    }

    private void ApplyKnockback(Collider target, float knockbackForce)
    {
        Rigidbody rb;
        rb = target.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            Vector3 dir = (target.transform.position - transform.position);
            dir.y = .5f;
            dir = dir.normalized;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }

    private void ApplyBurn(Collider target)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplyBurn(3f);
    }

    private void ApplySlow(Collider target, float duration, float speedMultiplier)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplySlow(duration, speedMultiplier);
    }

    private void ApplyRoot(Collider target)
    {
        if (!target.CompareTag("Enemy")) return;

        EnemyStatusReceiver status = target.GetComponentInParent<EnemyStatusReceiver>();
        status.ApplyRoot(2f);
    }
}
