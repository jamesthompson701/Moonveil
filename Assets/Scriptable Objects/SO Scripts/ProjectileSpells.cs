using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpell", menuName = "Scriptable Objects/Spells/ProjectileSpell")]
public class ProjectileSpells : SO_Spells
{
    [Header("Farm Overrides")]
    public float FarmSpeed = 10f;
    public float FarmLifetime = 1f;

    public override void CastSpell(SpellCastContext ctx)
    {
        if (SpellPrefab == null) return;

        Transform originT = ctx.attackCastOrigin != null ? ctx.attackCastOrigin : ctx.caster.transform;
        Vector3 origin = originT.position;

        Vector3 dir;
        float usedSpeed;
        float usedLifetime;
        float usedOffset;

        if (ctx.inCombatArea)
        {
            dir = (ctx.aimPoint - origin).normalized;
            usedSpeed = Speed;
            usedLifetime = Lifetime;
            usedOffset = ctx.combatSpawnOffset;
        }
        else
        {
            dir = originT.forward.normalized;
            usedSpeed = FarmSpeed;
            usedLifetime = FarmLifetime;
            usedOffset = ctx.farmSpawnOffset;
        }

        Vector3 spawnPos = origin + dir * usedOffset;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        Rigidbody clone = Spawn(SpellPrefab, spawnPos, rot);
        SetVelocity(clone, dir * usedSpeed);

        Destroy(clone.gameObject, usedLifetime);
    }
}
