using UnityEngine;

/// <summary>
/// Simple damage reference used by spells and enemy attacks. Does nothing by itself but is referenced by enemies and spells.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection, float impulseForce, GameObject instigator);
}

