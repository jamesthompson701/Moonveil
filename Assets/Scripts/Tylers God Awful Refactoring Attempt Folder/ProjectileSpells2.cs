using UnityEngine;

/// <summary>
/// Projectile spell with type-based effects (fire, water, air, earth).
/// spawns the projectile object and launches it forward then destroys it after time or contact with anything but the player
/// </summary>

[CreateAssetMenu(fileName = "ProjectileSpells2", menuName = "Scriptable Objects/ProjectileSpells2")]
public class ProjectileSpells2 : SO_SpellDefs2
{
    public override void CastSpell2(SpellCastContext ctx)
    {
        if (SpellPrefab == null) return;

        Transform originT = ctx.attackCastOrigin != null ? ctx.attackCastOrigin : ctx.caster.transform;
        Vector3 origin = originT.position + (originT.forward * forwardOffset) + (Vector3.up * upwardOffset) + (originT.right * horizontalOffset);

        Vector3 dir;
        float usedSpeed;
        float usedLifetime;
        float usedOffset;

        if (ctx.inCombatArea)
        {
            dir = (ctx.aimPoint - origin).normalized;
            usedSpeed = Speed;
            usedLifetime = Lifetime;
            usedOffset = ctx.spawnOffset;
        }
        else
        {
            dir = originT.forward.normalized;
            usedSpeed = Speed;
            usedLifetime = Lifetime;
            usedOffset = ctx.spawnOffset;
        }

        Vector3 spawnPos = origin + dir * usedOffset;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        Rigidbody clone = SpawnProjectile(SpellPrefab, spawnPos, rot);

        if (Speed == 0)
        {
            // Make the projectile follow the player's movement
            clone.transform.SetParent(ctx.caster.transform);
        }
        else
        {
            SetVelocity(clone, dir * usedSpeed);
        }

        SpellDamageManager2 dmg = clone.GetComponent<SpellDamageManager2>();
        if (dmg != null)
        {
            SpellManager sm = ctx.caster.GetComponent<SpellManager>();
            int choice = sm != null ? sm.attackChoice : 0;

            // Pass spell type and effects
            dmg.InitProjectile2(choice, damage, spellType, ctx.caster);
        }

        Destroy(clone.gameObject, usedLifetime);
    }
}
