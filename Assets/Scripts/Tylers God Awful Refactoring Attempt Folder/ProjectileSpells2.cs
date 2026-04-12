using UnityEngine;

/// <summary>
/// Projectile spell with type-based effects (fire, water, air, earth).
/// spawns the projectile object and launches it forward then destroys it after time or contact with anything but the player
/// </summary>

[CreateAssetMenu(fileName = "ProjectileSpells2", menuName = "Scriptable Objects/ProjectileSpells2")]
public class ProjectileSpells2 : SO_SpellDefs2
{
    // If true, spawned projectile will update its rotation to match the caster's look direction while active.
    public bool RotateWithCaster = true;

    public override void CastSpell2(SpellCastContext ctx)
    {
        if (SpellPrefab == null) return;

        Transform originT = ctx.attackCastOrigin != null ? ctx.attackCastOrigin : ctx.caster.transform;

        // Choose a forward vector for origin and default spawn direction that respects RotateWithCaster.
        Vector3 forwardForOffset = originT.forward;
        if (!RotateWithCaster)
        {
            // Ignore vertical look when RotateWithCaster is disabled: use horizontal forward
            forwardForOffset = Vector3.ProjectOnPlane(originT.forward, Vector3.up);
            if (forwardForOffset.sqrMagnitude < 0.0001f)
            {
                // Fallback to transform's local forward projected horizontally
                forwardForOffset = Vector3.ProjectOnPlane(originT.TransformDirection(Vector3.forward), Vector3.up);
            }
            forwardForOffset.Normalize();
        }

        Vector3 origin = originT.position + (forwardForOffset * forwardOffset) + (Vector3.up * upwardOffset) + (originT.right * horizontalOffset);

        Vector3 dir;
        float usedSpeed;
        float usedLifetime;
        float usedOffset;

        if (ctx.inCombatArea)
        {
            //dir = (ctx.aimPoint - origin).normalized;
            if (RotateWithCaster)
            {
                dir = originT.forward.normalized;
            }
            else
            {
                Vector3 horizontalForward = Vector3.ProjectOnPlane(originT.forward, Vector3.up);
                if (horizontalForward.sqrMagnitude < 0.0001f)
                {
                    horizontalForward = Vector3.ProjectOnPlane(originT.TransformDirection(Vector3.forward), Vector3.up);
                }
                dir = horizontalForward.normalized;
            }
            usedSpeed = Speed;
            usedLifetime = Lifetime;
            usedOffset = ctx.spawnOffset;
        }
        else
        {
            if (RotateWithCaster)
            {
                dir = originT.forward.normalized;
            }
            else
            {
                Vector3 horizontalForward = Vector3.ProjectOnPlane(originT.forward, Vector3.up);
                if (horizontalForward.sqrMagnitude < 0.0001f)
                {
                    horizontalForward = Vector3.ProjectOnPlane(originT.TransformDirection(Vector3.forward), Vector3.up);
                }
                dir = horizontalForward.normalized;
            }

            usedSpeed = Speed;
            usedLifetime = Lifetime;
            usedOffset = ctx.spawnOffset;
        }

        Vector3 spawnPos = origin + dir * usedOffset;

        // Block X rotation (pitch): compute yaw only and create a yaw-only rotation.
        Vector3 horizontalDirForRotation = Vector3.ProjectOnPlane(dir, Vector3.up);
        if (horizontalDirForRotation.sqrMagnitude < 0.0001f)
        {
            // fallback to caster horizontal forward if direction has tiny horizontal component
            horizontalDirForRotation = Vector3.ProjectOnPlane(originT.forward, Vector3.up);
            if (horizontalDirForRotation.sqrMagnitude < 0.0001f)
            {
                horizontalDirForRotation = Vector3.forward;
            }
        }
        float yaw = Mathf.Atan2(horizontalDirForRotation.x, horizontalDirForRotation.z) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, yaw, 0f);

        Rigidbody clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

        if (Speed == 0)
        {
            // Make the projectile follow the player's movement and spawn centered around the player
            clone.transform.SetParent(ctx.caster.transform);

            // Force the projectile to keep matching the caster's look direction while active
            if (RotateWithCaster)
            {
                var follower = clone.gameObject.AddComponent<FollowCasterRotation>();
                follower.Caster = ctx.caster.transform;
            }
        }
        else
        {
            SetVelocity(clone, dir * usedSpeed);
        }

        if (clone.TryGetComponent<SpellDamageManager2>(out var dmg))
        {

            // Pass spell type and effects
            dmg.InitProjectile2(damage, spellType);
        }

        Destroy(clone.gameObject, usedLifetime);
    }
}

/// <summary>
/// Small helper component added to spawned projectiles when RotateWithCaster is enabled.
/// Keeps the projectile's forward direction aligned with the caster's horizontal (yaw) direction each frame,
/// while blocking pitch (X rotation).
/// </summary>
public class FollowCasterRotation : MonoBehaviour
{
    public Transform Caster;

    void Update()
    {
        if (Caster == null) return;

        // Compute caster yaw (rotation around up).
        Vector3 casterForward = Vector3.ProjectOnPlane(Caster.forward, Vector3.up);
        if (casterForward.sqrMagnitude < 0.0001f) return; // no meaningful horizontal direction

        float casterYaw = Mathf.Atan2(casterForward.x, casterForward.z) * Mathf.Rad2Deg;

        // Block pitch: set rotation to yaw-only (X = 0), Z = 0 to avoid roll.
        transform.rotation = Quaternion.Euler(0f, casterYaw, 0f);
    }
}
