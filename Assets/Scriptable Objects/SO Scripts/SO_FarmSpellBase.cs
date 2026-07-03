using UnityEngine;

public abstract class SO_FarmSpellBase : SO_Spells
{
    [SerializeField] eEffects soundEffect;
    [Header("Farm Movement")]
    [SerializeField] protected float travelDistance = 10f;

    [Header("Spawn Height / Size")]
    [SerializeField] protected float heightOffset = 0f;     // keep 0 if farmCastOrigin already placed correctly
    [SerializeField] protected float scaleMultiplier = 1f;  // allow scaling up area later

    public override void CastSpell(SpellCastContext ctx)
    {
        Transform originT = ctx.farmCastOrigin != null ? ctx.farmCastOrigin : ctx.caster.transform;

        Vector3 forward = ctx.cameraPlanarForward;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;

        Vector3 spawnPos = originT.position + forward * ctx.farmSpawnOffset;
        spawnPos.y += heightOffset;

        Quaternion spawnRot = Quaternion.LookRotation(forward, Vector3.up);

        Rigidbody rb = Spawn(SpellPrefab, spawnPos, spawnRot);

        // scale for area changes (cloud size, etc.)
        if (rb != null)
            rb.transform.localScale *= scaleMultiplier;

        // movement handled by manager component
        FarmSpellMover mover = rb.GetComponent<FarmSpellMover>();
        if (mover == null) mover = rb.gameObject.AddComponent<FarmSpellMover>();

        mover.Init(forward, Speed, travelDistance, Lifetime, soundEffect);

        OnFarmSpellSpawned(rb, ctx);
    }

    // Hook for spell-specific extras later (VFX, audio, etc.)
    protected virtual void OnFarmSpellSpawned(Rigidbody spawned, SpellCastContext ctx) { }
}