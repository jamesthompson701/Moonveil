using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Attach to enemy melee hitboxes OR enemy projectile prefabs.
/// Handles damage with a short per-target cooldown.
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

    [Header("Target Filtering")]
    [Tooltip("Tags that this attack is allowed to damage. Default setup uses Player tag.")]
    [SerializeField] private string targetTag = "Player";

    [Header("Projectile Only")]
    [Tooltip("If true (projectiles), destroy on first valid hit.")]
    [SerializeField] private bool destroyOnHit = true;

    [Tooltip("Time before the projectile is automatically destroyed if it doesn't hit anything.")]
    [SerializeField] private float destroyDelay = 3f;

    private readonly Dictionary<int, float> _lastHitTime = new(8);
    private readonly HashSet<int> _hitThisSwing = new();

    [SerializeField] private bool isPenguinToss = false;
    [SerializeField] private GameObject penguinionPrefab;

    /// <summary>
    /// Call this right before enabling a melee hitbox for a new attack swing.
    /// </summary>

    public void ResetPerAttackMemory()
    {
        _hitThisSwing.Clear();
    }

    private void OnEnable()
    {
        if (IsMelee) _hitThisSwing.Clear();
    }

    private void Start()
    {
        if (!IsMelee && destroyDelay > 0f)
            Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hit = TryHit(other);
        if (IsMelee) return;                 
        if (!destroyOnHit) return;

        if (hit || other.CompareTag("Ground"))
        {
            SpawnPenguinion();
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);                       
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool hit = TryHit(collision.collider);
        if (IsMelee || !destroyOnHit) return;
        if (hit || collision.collider.CompareTag("Ground"))
        {
            SpawnPenguinion();
            Destroy(gameObject);
        }
    }

    private bool TryHit(Collider other)
    {
        if (!other) return false;
        if (!string.IsNullOrWhiteSpace(targetTag) && !other.CompareTag(targetTag)) return false;

        int targetId = other.transform.root.GetInstanceID();
        if (IsMelee && _hitThisSwing.Contains(targetId)) return false;

        if (_lastHitTime.TryGetValue(targetId, out float last) &&
            Time.time - last < damageCooldownSeconds) return false;

        PlayerDamageReceiver receiver = other.GetComponentInParent<PlayerDamageReceiver>();
        if (!receiver) return false;

        receiver.TakeDamage(Damage);
        _lastHitTime[targetId] = Time.time;
        if (IsMelee) _hitThisSwing.Add(targetId);
        return true;
    }

    private void SpawnPenguinion()
    {
        if (!isPenguinToss) return;
        PengKingBoss boss = Object.FindFirstObjectByType<PengKingBoss>();
        for (int i = 0; i < boss.maxSpawnedMinions; i++)
        {
            Instantiate(penguinionPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            boss.RegisterSpawnedMinion();
        }

        Debug.Log("Spawning Penguinion!");
    }
}

