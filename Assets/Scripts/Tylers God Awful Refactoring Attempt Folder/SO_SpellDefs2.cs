using UnityEngine;

/// <summary>
/// Base class for spell definitions.
/// should contain any shared variables such as postiion, rotation, scale, prefab reference, cast point reference, etc.
/// </summary>

[CreateAssetMenu(fileName = "SO_SpellDefs2", menuName = "Scriptable Objects/SO_SpellDefs2")]
public abstract class SO_SpellDefs2 : ScriptableObject
{
    public enum SpellType
    {
        Fire = 0,
        Earth = 1,
        Water = 2,
        Air = 3
    }

    [Header("Spell Type")]
    [Tooltip("Element/type used for resource pools and cooldown pools.")]
    public SpellType spellType;

    [Header("Info")]
    public string SpellName;

    [Header("Prefab")]
    public Rigidbody SpellPrefab;

    [Header("Stats")]
    public int damage;
    public float Speed = 15f;

    [Header("Lifetime")]
    public float Lifetime = 3f;

    [Header("Preview")]
    [Tooltip("Prefab for the preview of the spell.")]
    [SerializeField] public GameObject previewPrefab;

    [Header("Placement Rules")]
    [Tooltip("Tag required for valid ground placement.")]
    [SerializeField] public string groundTag = "Ground";

    public float forwardOffset = 1f;
    public float upwardOffset = 0f;
    public float horizontalOffset = 0f;

    public abstract void CastSpell2(SpellCastContext ctx);

    protected Rigidbody SpawnProjectile(Rigidbody prefab, Vector3 pos, Quaternion rot)
    {
        return Instantiate(prefab, pos, rot);
    }

    protected GameObject SpawnGroundEffect(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        return Instantiate(prefab, pos, rot);
    }

    protected GameObject SpawnStream(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject instance = Instantiate(prefab, pos, rot, parent);
        return instance;
    }

    protected void SetVelocity(Rigidbody rb, Vector3 vel)
    {
        rb.linearVelocity = vel;
    }
}
