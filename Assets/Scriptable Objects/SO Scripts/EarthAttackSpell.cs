using UnityEngine;

[CreateAssetMenu(fileName = "EarthAttackSpell", menuName = "Scriptable Objects/EarthAttackSpell")]
public class EarthAttackSpell : SO_Spells
{
    [Header("Placement Rules")]
    [SerializeField] private string groundTag = "Ground";

    [Header("Growth")]
    public float AddY = 2f;
    public float MaxY = 5f;

    public override void CastSpell(SpellCastContext ctx)
    {
        if (SpellPrefab == null) return;

        Transform originT = ctx.attackCastOrigin != null ? ctx.attackCastOrigin : ctx.caster.transform;

        if (ctx.inCombatArea)
        {
            // Must hit something
            if (!ctx.hasHit || ctx.hitCollider == null)
            {
                Debug.Log("Earth attack needs a ground hit to place correctly.");
                return;
            }

            // ONLY allow Ground tag
            if (!ctx.hitCollider.CompareTag(groundTag))
            {
                Debug.Log("Earth attack can only be placed on Ground.");
                return;
            }

            // Align earth "up" to the surface normal
            Quaternion earthRot = Quaternion.FromToRotation(Vector3.up, ctx.aimNormal);

            // Find half-height of the prefab so we can place it ON TOP of the ground
            float halfHeight = 0.5f;
            Collider prefabCol = SpellPrefab.GetComponent<Collider>();
            if (prefabCol != null)
                halfHeight = prefabCol.bounds.extents.y;

            // Start position: on the surface, pushed out by half height so it isn't inside the ground
            Vector3 earthPos = ctx.aimPoint + ctx.aimNormal * halfHeight;

            Rigidbody clone = Spawn(SpellPrefab, earthPos, earthRot);
            SetVelocity(clone, Vector3.zero);

            // Grow upward: increase Y and push up by half the added amount so bottom stays planted
            Vector3 s = clone.transform.localScale;
            float oldY = s.y;

            s.y = Mathf.Min(s.y + AddY, MaxY);
            clone.transform.localScale = s;

            float deltaY = s.y - oldY;

            // move up along the surface normal so growth doesn't sink into the ground
            if (deltaY > 0f)
                clone.transform.position += ctx.aimNormal * (deltaY * 0.5f);

            Destroy(clone.gameObject, Lifetime);
        }
        else
        {
            // Farm: simple place in front
            Vector3 dir = originT.forward.normalized;
            Vector3 pos = originT.position + dir * ctx.farmSpawnOffset;
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            Rigidbody clone = Spawn(SpellPrefab, pos, rot);
            SetVelocity(clone, Vector3.zero);
            Destroy(clone.gameObject, 1f);
        }
    }
}
