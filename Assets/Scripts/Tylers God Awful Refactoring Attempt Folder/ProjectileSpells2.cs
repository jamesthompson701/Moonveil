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

        // REQUIRE spawn anchor: per request, prefab must spawn at the castOrigin.
        if (ctx.castOrigin == null)
        {
            Debug.LogWarning("ProjectileSpells2.CastSpell2: castOrigin is null — spawn aborted.");
            return;
        }
        if (Speed == 0)
        {
            ctx.castOrigin = SpellManager2.Instance.stationaryCastOrigin;
        }
        Transform originT = ctx.castOrigin;

        // Choose the transform to derive yaw (horizontal) axes from.
        // When RotateWithCaster is enabled we want offsets (especially horizontalOffset) to be relative to the caster's yaw,
        // not the camera/castOrigin, which prevents camera facing left/right from shifting the spawn laterally.
        Transform basisForYaw = (RotateWithCaster && ctx.caster != null) ? ctx.caster.transform : originT;

        // Compute yaw-only forward and right vectors from the chosen basis.
        Vector3 yawForward = Vector3.ProjectOnPlane(basisForYaw.forward, Vector3.up);
        if (yawForward.sqrMagnitude < 0.0001f)
        {
            yawForward = Vector3.ProjectOnPlane(basisForYaw.TransformDirection(Vector3.forward), Vector3.up);
            if (yawForward.sqrMagnitude < 0.0001f)
            {
                yawForward = Vector3.forward;
            }
        }
        yawForward.Normalize();
        Quaternion yawRotation = Quaternion.LookRotation(yawForward, Vector3.up);
        Vector3 yawRight = yawRotation * Vector3.right;

        // Use yaw-only forward for offset computation to avoid pitch/roll affecting spawn position.
        Vector3 forwardForOffset = yawForward;

        // Base origin is exactly the cast origin position, then apply configured offsets from SO_SpellDefs2
        Vector3 origin = originT.position + (forwardForOffset * forwardOffset) + (Vector3.up * upwardOffset) + (yawRight * horizontalOffset);

        // Decide firing direction.
        // If RotateWithCaster is true, use caster yaw-only forward (so spawn & initial facing ignore camera pitch).
        // If false, use cast origin horizontal forward (so aim/placement follows castOrigin's planar forward).
        Vector3 dir;
        if (RotateWithCaster)
        {
            dir = yawForward;
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

        float usedSpeed = Speed;
        float usedLifetime = Lifetime;
        float usedOffset = ctx.spawnOffset;

        Vector3 spawnPos = origin + dir * usedOffset;

        // Create a yaw-only rotation for the projectile so it isn't pitched by the camera.
        float yaw = Mathf.Atan2(yawForward.x, yawForward.z) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, yaw, 0f);

        Rigidbody clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

        if (usedSpeed == 0)
        {
            // Make the projectile follow the player's movement and spawn centered around the player
            if (ctx.caster != null)
                clone.transform.SetParent(ctx.caster.transform);

            // Force the projectile to keep matching the caster's look direction while active
            if (RotateWithCaster && ctx.caster != null)
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
/// Prevents the projectile from spawning at a different pitch based on the caster's look direction at cast time, and keeps it aligned with the caster if they look around while the projectile is active.
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
