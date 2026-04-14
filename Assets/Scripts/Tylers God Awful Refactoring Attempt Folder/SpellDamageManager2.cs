using System.Collections;
using System.Collections.Generic;
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
    [SerializeField, Tooltip("How much slow is applied.")] private float slowAmount;
    private SO_SpellDefs2.SpellType spellType;
    [SerializeField] private bool isProjectile = false;
    [SerializeField] private bool isBasicAttack = false;
    private CreatureDefs _creatureDefs;
    
    [Header("Status Effects")]
    [SerializeField] private bool appliesBurn;
    [SerializeField] private bool appliesSlow;
    [SerializeField] private bool appliesRoot;
    [SerializeField] private bool appliesKnockback;
    [SerializeField, Tooltip("Aplies overlap sphere effect. Not the same as water Tier 2-4")] private bool appliesAOE;
    [SerializeField, Tooltip("Use on Water Tier 2-4 Combat to do damage to enemies that linger in the attack")] private bool appliesDamageOverTime;

    // Track which enemies are already being damaged over time
    private readonly HashSet<CreatureDefs> _dotActive = new HashSet<CreatureDefs>();

    public void InitProjectile2(int dmg, SO_SpellDefs2.SpellType type)
    {
        damage = dmg;
        spellType = type;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _creatureDefs = other.GetComponentInParent<CreatureDefs>();
        }

        if (other.CompareTag("Ground"))
        {
            if (appliesAOE)
            {
                var alreadyHit = new HashSet<CreatureDefs>();
                Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        var creature = hit.GetComponentInParent<CreatureDefs>();
                        if (creature != null && !alreadyHit.Contains(creature))
                        {
                            alreadyHit.Add(creature);

                            if (appliesBurn)
                                creature.ApplyBurn(duration);
                            if (appliesSlow)
                                creature.ApplySlow(duration);
                            if (appliesRoot)
                                creature.ApplyRoot(duration);
                            ApplyDamage(hit, aoeDamage);
                            if (appliesKnockback)
                                ApplyKnockback(hit, force);
                        }
                    }
                }
            }
            if (isProjectile)
                Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Enemy")) return;

        if (_creatureDefs != null)
        {
            ApplyDamage(other, damage);

            if (appliesBurn)
                _creatureDefs.ApplyBurn(duration);
            if (appliesSlow)
                _creatureDefs.ApplySlow(duration);
            if (appliesRoot)
                _creatureDefs.ApplyRoot(duration);
            if (appliesKnockback)
                ApplyKnockback(other, force);
        }

        if (appliesAOE)
        {
            var alreadyHit = new HashSet<CreatureDefs>();
            alreadyHit.Add(_creatureDefs); // Prevent double-hit on the initial target

            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            foreach (Collider hit in hits)
            {
                if (hit != other && hit.CompareTag("Enemy"))
                {
                    var creature = hit.GetComponentInParent<CreatureDefs>();
                    if (creature != null && !alreadyHit.Contains(creature))
                    {
                        alreadyHit.Add(creature);

                        ApplyDamage(hit, aoeDamage);
                        if (appliesBurn)
                            creature.ApplyBurn(duration);
                        if (appliesSlow)
                            creature.ApplySlow(duration);
                        if (appliesRoot)
                            creature.ApplyRoot(duration);
                        if (appliesKnockback)
                            ApplyKnockback(hit, force);
                    }
                }
            }
        }

        if (isProjectile)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (appliesDamageOverTime)
        {
            var creature = other.GetComponentInParent<CreatureDefs>();
            if (creature != null && !_dotActive.Contains(creature))
            {
                _dotActive.Add(creature);
                StartCoroutine(ApplyDamageInIntervalsCoroutine(creature, damage, 1f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (appliesDamageOverTime)
        {
            var creature = other.GetComponentInParent<CreatureDefs>();
            if (creature != null)
            {
                _dotActive.Remove(creature);
            }
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
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (target.transform.position - transform.position);
            dir.y = .5f;
            dir = dir.normalized;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }

    // Coroutine for DoT: deals damage every interval while the enemy is in the collider
    private IEnumerator ApplyDamageInIntervalsCoroutine(CreatureDefs creature, float damagePerTick, float interval)
    {
        while (_dotActive.Contains(creature))
        {
            creature.TakeDamage(damagePerTick, null);
            yield return new WaitForSeconds(interval);
        }
    }
}
