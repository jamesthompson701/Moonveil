using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Attach to enemy melee hitboxes OR enemy projectile prefabs.
/// Handles damage + stun pushback with a short per-target cooldown.
/// </summary>
public class EnemyAttacks : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("How much damage this attack deals per successful hit.")]
    public float Damage = 10f;

    [FormerlySerializedAs("isMelee")]
    [Tooltip("If true, treat collider as a melee hitbox (usually trigger). If false, treat as a projectile.")]
    public bool IsMelee = false;

    [Tooltip("Seconds before the same target can be damaged again by this same attack object.")]
    [SerializeField, Min(0f)] private float damageCooldownSeconds = 0.35f;

    [Header("Stun Push")]
    [Tooltip("Impulse applied to the player on hit (handled through PlayerImpactReceiver for CharacterController).")]
    [SerializeField, Min(0f)] private float stunImpulse = 3.0f;

    [Header("Target Filtering")]
    [Tooltip("Tags that this attack is allowed to damage. Default setup uses Player tag.")]
    [SerializeField] private string targetTag = "Player";

    [Header("Projectile Only")]
    [Tooltip("If true (projectiles), destroy on first valid hit.")]
    [SerializeField] private bool destroyOnHit = true;

    private readonly Dictionary<int, float> _lastHitTime = new Dictionary<int, float>(8);
    private readonly HashSet<int> _hitThisSwing = new HashSet<int>();

    /// <summary>
    /// Call this right before enabling a melee hitbox for a new attack swing.
    /// </summary>
    public void ResetPerAttackMemory()
    {
        _hitThisSwing.Clear();
    }

    private void OnTriggerEnter(Collider other) => TryHit(other);
    private void OnTriggerStay(Collider other) => TryHit(other);
    private void OnCollisionEnter(Collision collision) => TryHit(collision.collider);

    private void TryHit(Collider other)
    {
        if (!other) return;

        if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag))
            return;

        int targetId = other.transform.root.GetInstanceID();

        if (IsMelee && _hitThisSwing.Contains(targetId))
            return;

        if (_lastHitTime.TryGetValue(targetId, out float last))
        {
            if (Time.time - last < damageCooldownSeconds)
                return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 dir = Vector3.ProjectOnPlane(other.transform.position - transform.position, Vector3.up).normalized;

        if (damageable != null)
            damageable.TakeDamage(Damage, hitPoint, dir, 0f, gameObject);
        else
        {
            PlayerDamageReceiver receiver = other.GetComponentInParent<PlayerDamageReceiver>();
            if (receiver) receiver.TakeDamage(Damage);
        }

        PlayerImpactReceiver impact = other.GetComponentInParent<PlayerImpactReceiver>();
        if (impact && stunImpulse > 0f)
            impact.AddImpulse(dir * stunImpulse);

        _lastHitTime[targetId] = Time.time;
        if (IsMelee) _hitThisSwing.Add(targetId);

        if (!IsMelee && destroyOnHit)
            Destroy(gameObject);
    }
}

