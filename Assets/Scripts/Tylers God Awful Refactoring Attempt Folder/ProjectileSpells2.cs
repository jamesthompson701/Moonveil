using UnityEngine;

/// <summary>
/// Projectile spell with type-based effects (fire, water, air, earth).
/// Supports three spawn modes:
///  - Fired: instant projectile fired in the camera/player look direction and NOT parented (doesn't follow player).
///  - SpawnFixedToCaster: spawns on the caster, follows the caster's position but keeps the rotation fixed to the cast rotation.
///  - SpawnFollowCaster: spawns on the caster and rotates with the caster.
/// </summary>
[CreateAssetMenu(fileName = "ProjectileSpells2", menuName = "Scriptable Objects/ProjectileSpells2")]
public class ProjectileSpells2 : SO_SpellDefs2
{
    public enum ProjectileSpawnMode
    {
        Fired = 0,
        SpawnFixedToCaster = 1,
        SpawnFollowCaster = 2
    }

    [Tooltip("Controls which of the three projectile behaviours to use.")]
    public ProjectileSpawnMode SpawnMode = ProjectileSpawnMode.Fired;

    // If true, spawned projectile will update its rotation to match the caster's look direction while active (used only if you add that behaviour).
    public bool RotateWithCaster = true;

    public override void CastSpell2(SpellCastContext ctx)
    {
        // Resolve origins and fallbacks robustly
        Transform originT = ctx.castOrigin;
        if (originT == null)
            originT = SpellManager2.Instance?.projectilCastOrigin ?? ctx.caster?.transform ?? SpellManager2.Instance?.player?.transform;

        if (originT == null)
        {
            Debug.LogWarning("ProjectileSpells2.CastSpell2: no valid cast origin or caster found — spawn aborted.");
            return;
        }

        float usedSpeed = Speed;
        float usedLifetime = Lifetime;
        float usedOffset = ctx.spawnOffset;

        // Common: compute a yaw-based forward from the caster or origin for positional offsets where appropriate.
        Transform basisForYaw = (RotateWithCaster && ctx.caster != null) ? ctx.caster.transform : originT;
        Vector3 yawForward = Vector3.ProjectOnPlane(basisForYaw.forward, Vector3.up);
        if (yawForward.sqrMagnitude < 0.0001f)
        {
            yawForward = Vector3.ProjectOnPlane(basisForYaw.TransformDirection(Vector3.forward), Vector3.up);
            if (yawForward.sqrMagnitude < 0.0001f)
                yawForward = Vector3.forward;
        }
        yawForward.Normalize();

        Rigidbody clone = null;
        Vector3 spawnPos = originT.position + yawForward * forwardOffset + Vector3.up * upwardOffset + (Quaternion.LookRotation(yawForward) * Vector3.right) * horizontalOffset;
        Quaternion rot = Quaternion.identity;

        switch (SpawnMode)
        {
            case ProjectileSpawnMode.Fired:
                {
                    // Fired: full 3D camera/caster look direction (preserve pitch). Not parented so movement of player won't change projectile position.
                    Vector3 fireDir = ctx.aimCamera != null ? ctx.aimCamera.transform.forward : (ctx.caster != null ? ctx.caster.transform.forward : yawForward);
                    if (fireDir.sqrMagnitude < 0.0001f) fireDir = yawForward;
                    fireDir.Normalize();

                    spawnPos = originT.position + fireDir * usedOffset; // offset along aim direction
                    rot = Quaternion.LookRotation(fireDir, Vector3.up);

                    clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

                    // Apply velocity (if speed == 0 then projectile will be stationary; still not parented)
                    if (usedSpeed != 0f)
                        SetVelocity(clone, fireDir * usedSpeed);

                    break;
                }
            case ProjectileSpawnMode.SpawnFixedToCaster:
                {
                    // Spawn on the caster and keep the projectile facing the direction they cast in.
                    // Position follows caster, rotation remains fixed in world-space to the cast rotation.
                    Vector3 castFacing = ctx.aimCamera != null ? ctx.aimCamera.transform.forward : (ctx.caster != null ? ctx.caster.transform.forward : yawForward);
                    if (castFacing.sqrMagnitude < 0.0001f) castFacing = yawForward;
                    castFacing.Normalize();

                    spawnPos = (ctx.caster != null ? ctx.caster.transform.position : originT.position) + castFacing * forwardOffset + Vector3.up * upwardOffset;
                    rot = Quaternion.LookRotation(castFacing, Vector3.up);

                    clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

                    // Parent for position following but preserve world rotation via helper component
                    if (ctx.caster != null)
                    {
                        clone.transform.SetParent(ctx.caster.transform, true);
                        var keepRot = clone.gameObject.AddComponent<KeepWorldRotationOnParent>();
                        keepRot.FixedWorldRotation = rot;
                    }

                    // Ensure stationary behaviour (0 speed)
                    if (usedSpeed != 0f)
                        SetVelocity(clone, Vector3.zero);

                    break;
                }
            case ProjectileSpawnMode.SpawnFollowCaster:
                {
                    // Spawn on caster and rotate with them. If usedSpeed > 0 this behaves like a projectile attached to the caster.
                    spawnPos = (ctx.caster != null ? ctx.caster.transform.position : originT.position) + yawForward * forwardOffset + Vector3.up * upwardOffset;
                    rot = (ctx.caster != null) ? ctx.caster.transform.rotation : Quaternion.LookRotation(yawForward, Vector3.up);

                    clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

                    if (ctx.caster != null)
                    {
                        clone.transform.SetParent(ctx.caster.transform, true);
                    }

                    // If RotateWithCaster is true and you want yaw-only locking, we could add the old helper; otherwise parent provides full rotation following.
                    if (usedSpeed != 0f)
                    {
                        // If speed > 0 treat as a projectile launched from the caster's facing (but still parented if desired)
                        SetVelocity(clone, clone.transform.forward * usedSpeed);
                    }

                    break;
                }
        }

        if (clone == null)
        {
            Debug.LogWarning("ProjectileSpells2.CastSpell2: spawn failed (clone is null)");
            return;
        }

        if (clone.TryGetComponent<SpellDamageManager2>(out var dmg))
        {
            // Pass spell type and effects
            dmg.InitProjectile2(damage, spellType);
        }

        playSpellAudio();

        // schedule destroy
        Destroy(clone.gameObject, usedLifetime);
    }
}

/// <summary>
/// Keeps a spawned projectile's world rotation fixed even when the object is parented to a moving/rotating caster.
/// Useful for 'SpawnFixedToCaster' behaviour: projectile follows position but keeps the rotation it had at spawn.
/// </summary>
public class KeepWorldRotationOnParent : MonoBehaviour
{
    [HideInInspector]
    public Quaternion FixedWorldRotation = Quaternion.identity;

    void LateUpdate()
    {
        transform.rotation = FixedWorldRotation;
    }
}
