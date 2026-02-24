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

        SpellDamageManager dmg = clone.GetComponent<SpellDamageManager>();
        if (dmg != null)
        {
            SpellManager sm = ctx.caster.GetComponent<SpellManager>();
            int choice = sm != null ? sm.attackChoice : 0;

            // Use SO damage, and a reasonable force (or add Force to SO later)
            dmg.Init(choice, damage, 12f);
        }

        Destroy(clone.gameObject, usedLifetime);
    }  
}
