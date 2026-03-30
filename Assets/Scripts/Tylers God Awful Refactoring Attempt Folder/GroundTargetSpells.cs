using UnityEngine;

/// <summary>
/// Ground target spell with type-based effects (fire, water, air, earth).
/// Should work by spawning the spell prefab at the target location and applying effects to enemies in the area
/// This should be done by raycast to the ground to find the target location, then spawning the prefab there and applying effects to enemies in the area
/// </summary>

[CreateAssetMenu(fileName = "GroundTargetSpells", menuName = "Scriptable Objects/GroundTargetSpells")]
public class GroundTargetSpells : SO_SpellDefs2
{
    //TODO
    //find prefab
    //find cast point
    //raycast to ground to find target location
    //Show preveiw of spell area of effect at target location while aiming
    //instantiate or enable prefab/object at target location
    //apply effects to enemies in area of effect
    //destroy or disable prefab/object when spell ends

    public override void CastSpell2(SpellCastContext ctx)
    {
        if (!TryGetAimHit(ctx, out RaycastHit hit))
            return;

        // targets the ground 
        if (!string.IsNullOrWhiteSpace(groundTag) && !hit.collider.CompareTag(groundTag))
            return;

        float yOffset = GroundYOffset;
        Vector3 spawnPos = hit.point + (Vector3.up * yOffset);

        Quaternion rot = Quaternion.identity;
        if (AlignToSurfaceNormal)
        {
            // Align "up" to the surface normal; maintain camera forward projection as forward.  
            Vector3 forwardProjected = Vector3.ProjectOnPlane(ctx.cameraPlanarForward, hit.normal).normalized;
            if (forwardProjected.sqrMagnitude < 0.001f)
                forwardProjected = Vector3.Cross(hit.normal, Vector3.right);

            rot = Quaternion.LookRotation(forwardProjected, hit.normal);
        }
        else
        {
            rot = Quaternion.LookRotation(ctx.cameraPlanarForward, Vector3.up);
        }

        rot *= Quaternion.Euler(RotationOffsetEuler);

        GameObject spawned = Instantiate(SpellPrefab.gameObject, spawnPos, rot);

        if (spawned.TryGetComponent<SpellDamageManager2>(out var dmg))
        {
            SpellManager2 sm = ctx.caster.GetComponent<SpellManager2>();
            int choice = sm != null ? sm.attackChoice : 0;

            if (ctx.caster != null)
                dmg.InitProjectile2(choice, damage, spellType, ctx.caster);
        }

        if (Lifetime > 0f)
            Destroy(spawned, Lifetime);
    }

    [Header("Earth Placement")]
    [Tooltip("Small upward offset applied to the preview and spawned Earth prefab.\n" +
             "Prevents z-fighting and reduces chance of the prefab intersecting the ground collider.")]
    [Min(0f)]
    [SerializeField] private float groundYOffset = 0.02f;

    [Tooltip("If true, the preview/spawn rotates to match the surface normal of the raycast hit.")]
    [SerializeField] private bool alignToSurfaceNormal = true;

    [Tooltip("Additional rotation (degrees) applied after alignment.\n" +
             "Useful if your Earth prefab faces a different forward axis.")]
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    public float GroundYOffset => groundYOffset;
    public bool AlignToSurfaceNormal => alignToSurfaceNormal;
    public Vector3 RotationOffsetEuler => rotationOffsetEuler;

    private bool TryGetAimHit(SpellCastContext ctx, out RaycastHit hit)
    {
        return Physics.Raycast(ctx.aimCamera.transform.position, ctx.aimCamera.transform.forward, out hit, ctx.aimDistance, ctx.aimMask);
    }
}

