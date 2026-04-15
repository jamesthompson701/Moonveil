using UnityEngine;

public class EnemyStatusReceiver : MonoBehaviour
{
    //[Header("Immunity Settings")]
    //[SerializeField] private bool _canBurn;
    //[SerializeField] private bool _canSlow;
    //[SerializeField] private bool _canRoot;
    //
    //private CreatureDefs _creatureDefs;
    //
    //private void Awake()
    //{
    //    _creatureDefs = GetComponent<CreatureDefs>();
    //}
    //
    //private void Update()
    //{
    //    float dt = Time.deltaTime;
    //
    //    UpdateBurn(dt);
    //    UpdateSlow(dt);
    //}
    //
    //// Handles Applying the status effects
    //public void ApplyBurn(float durationSeconds)
    //{
    //    Debug.Log("Applying burn for " + durationSeconds + " seconds.");
    //    if (_isBurning)
    //    {
    //        _burnRemaining = Mathf.Max(_burnRemaining, durationSeconds); 
    //        return;
    //    }
    //
    //    if (!_canBurn) return;
    //
    //    // Refresh duration; keep the higher dps if re-applied.
    //    _burnRemaining = Mathf.Max(_burnRemaining, durationSeconds);
    //    _isBurning = true;
    //}
    //
    //public void ApplySlow(float durationSeconds, float speedMultiplier)
    //{
    //    Debug.Log("Applying slow for " + durationSeconds + " seconds with speed multiplier " + speedMultiplier);
    //    if (durationSeconds <= 0f || !_canSlow)
    //        return;
    //
    //    _slowRemaining = Mathf.Max(_slowRemaining, durationSeconds);
    //    _slowSpeedMultiplier = Mathf.Clamp(speedMultiplier, 0f, 1f);
    //
    //    if (!_slowApplied)
    //        SetSlowActive(true);
    //}
    //
    //public void ApplyRoot(float durationSeconds)
    //{
    //    Debug.Log("Applying root for " + durationSeconds + " seconds.");
    //    if (durationSeconds <= 0f || !_canRoot)
    //        return;
    //    ApplySlow(durationSeconds, 0f); // Full slow (no movement) for root
    //}
    //
    //// Implementation and updating of the status effects
    //private void UpdateBurn(float dt)
    //{
    //    if (_burnRemaining <= 0f)
    //        return;
    //
    //    _burnRemaining -= dt;
    //
    //    float damageThisFrame = _burnDps * dt;
    //
    //    if (_creatureDefs != null)
    //    {
    //        _creatureDefs.TakeDamage(damageThisFrame, null);
    //    }
    //
    //    if (_burnRemaining <= 0f)
    //    {
    //        _burnRemaining = 0f;
    //    }
    //}
    //
    //private void UpdateSlow(float dt)
    //{
    //    if (_slowRemaining <= 0f)
    //        return;
    //
    //    _slowRemaining -= dt;
    //
    //    if (_slowRemaining <= 0f)
    //    {
    //        _slowRemaining = 0f;
    //        SetSlowActive(false);
    //    }
    //}
    //
    //private void SetSlowActive(bool active)
    //{
    //    _slowApplied = active;
    //
    //    if (_creatureDefs != null)
    //    {
    //        if (active)
    //        {
    //            _creatureDefs.maxSpeed *= _slowSpeedMultiplier;
    //        }
    //    }
    //}
}
