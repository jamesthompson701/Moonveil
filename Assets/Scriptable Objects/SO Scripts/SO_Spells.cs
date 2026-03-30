using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spells", menuName = "Scriptable Objects/SO_Spells")]
public abstract class SO_Spells : ScriptableObject
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

    [Header("Farm Overrides")]
    public float FarmSpeed = 10f;
    public float FarmLifetime = 1f;

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
    [Tooltip("Prefab for the preview spell.")]
    [SerializeField] public GameObject previewPrefab;
    public GameObject previewInstance;

    [Header("Placement Rules")]
    [Tooltip("Tag required for valid ground placement.")]
    [SerializeField] public string groundTag = "Ground";

    public abstract void CastSpell(SpellCastContext ctx);

    protected Rigidbody Spawn(Rigidbody prefab, Vector3 pos, Quaternion rot)
    {
        return Instantiate(prefab, pos, rot);
    }

    protected void SetVelocity(Rigidbody rb, Vector3 vel)
    {
        rb.linearVelocity = vel;
    }
}
