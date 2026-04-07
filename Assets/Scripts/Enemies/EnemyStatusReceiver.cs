using UnityEngine;

[DisallowMultipleComponent]
public class EnemyStatusReceiver : MonoBehaviour
{
    [Header("Optional References")]
    [Tooltip("If present, forces/status effects can tweak physics on this Rigidbody.\n" +
             "If left null, we will try to auto-find one on this GameObject.")]
    [SerializeField] private Rigidbody cachedRigidbody;

    [Header("Stun Behavior")]
    [Tooltip("Optional list of MonoBehaviours to disable while stunned.\n" +
             "Example: your enemy AI script, NavMesh movement script, attack script, etc.")]
    [SerializeField] private MonoBehaviour[] disableWhileStunned;

    // Burn state
    private float _burnRemaining;
    private float _burnDps;
    private Transform _burnInstigator;

    // Slow state
    private float _slowRemaining;
    private float _slowSpeedMultiplier = 1f;
    private bool _slowApplied;

    private CreatureDefs _creatureDefs;

    private void Awake()
    {
        if (cachedRigidbody == null)
            cachedRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        UpdateBurn(dt);
        UpdateSlow(dt);
    }

    private void DealDamage(float amount, Transform instigator)
    {
        if (amount <= 0f)
            return;

        if (TryGetComponent<IDamageable>(out var damageable))
        {
            Vector3 hitDirection = Vector3.zero; // Default or calculated direction
            Vector3 hitPoint = transform.position; // Default or calculated hit point
            float force = 0f; // Default or calculated force
            GameObject source = instigator != null ? instigator.gameObject : null;

            damageable.TakeDamage(amount, hitDirection, hitPoint, force, source);
            return;
        }
    }

    // Handles Applying the status effects
    public void ApplyBurn(float durationSeconds, float dps, Transform instigator)
    {
        if (durationSeconds <= 0f || dps <= 0f)
            return;

        // Refresh duration; keep the higher dps if re-applied.
        _burnRemaining = Mathf.Max(_burnRemaining, durationSeconds);
        _burnDps = Mathf.Max(_burnDps, dps);
        _burnInstigator = instigator;
    }

    public void ApplySlow(float durationSeconds, float speedMultiplier)
    {
        if (durationSeconds <= 0f)
            return;

        _slowRemaining = Mathf.Max(_slowRemaining, durationSeconds);
        _slowSpeedMultiplier = Mathf.Clamp(speedMultiplier, 0f, 1f);

        if (!_slowApplied)
            SetSlowActive(true);
    }

    public void ApplyRoot(float durationSeconds)
    {
        if (durationSeconds <= 0f)
            return;
        ApplySlow(durationSeconds, 0f); // Full slow (no movement) for root
    }

    // Implementation and updating of the status effects
    private void UpdateBurn(float dt)
    {
        if (_burnRemaining <= 0f)
            return;

        _burnRemaining -= dt;

        float damageThisFrame = _burnDps * dt;
        DealDamage(damageThisFrame, _burnInstigator);

        if (_burnRemaining <= 0f)
        {
            _burnRemaining = 0f;
            _burnInstigator = null;
        }
    }

    private void UpdateSlow(float dt)
    {
        if (_slowRemaining <= 0f)
            return;

        _slowRemaining -= dt;

        if (_slowRemaining <= 0f)
        {
            _slowRemaining = 0f;
            SetSlowActive(false);
        }
    }

    private void SetSlowActive(bool active)
    {
        _slowApplied = active;

        if (_creatureDefs != null)
        {
            if (active)
            {
                _creatureDefs.maxSpeed *= _slowSpeedMultiplier;
            }
        }
    }
}
