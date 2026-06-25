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
    public GameObject previewPrefab;

    [Header("Placement Rules")]
    [Tooltip("Tag required for valid ground placement.")]
    public string groundTag = "Ground";
    public string soilTag = "Soil";

    public float forwardOffset = 1f;
    public float upwardOffset = 0f;
    public float horizontalOffset = 0f;

    // Make audio source optional and don't reference SpellManager2.Instance during asset construction.
    [Tooltip("Optional transform used as audio source. If null, SpellManager2.Instance.player.transform will be used at runtime if available.")]
    public Transform audioSource;

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

    public void playSpellAudio()
    {
        // Resolve manager and audio source at runtime and guard against nulls.
        var mgr = SpellManager2.Instance;
        Transform src = audioSource ?? mgr?.player?.transform;

        if (mgr == null)
        {
            Debug.LogWarning("playSpellAudio: SpellManager2.Instance is null. Audio aborted.");
            return;
        }

        if (src == null)
        {
            Debug.LogWarning("playSpellAudio: audio source is null. Audio aborted.");
            return;
        }

        bool inCombat = mgr.inCombatArea;

        if (spellType == SpellType.Fire && inCombat)
            AudioManager.PlayOneShot(eEffects.combatFire, src, 100);
        else if (spellType == SpellType.Earth && inCombat)
            AudioManager.PlayOneShot(eEffects.combatEarth, src, 100);
        else if (spellType == SpellType.Water && inCombat)
            AudioManager.PlayOneShot(eEffects.combatWater, src, 100);
        else if (spellType == SpellType.Air && inCombat)
            AudioManager.PlayOneShot(eEffects.combatAir, src, 100);

        if (spellType == SpellType.Fire && !inCombat)
            AudioManager.PlayOneShot(eEffects.farmFire, src, 100);
        else if (spellType == SpellType.Earth && !inCombat)
            AudioManager.PlayOneShot(eEffects.farmEarth, src, 100);
        else if (spellType == SpellType.Water && !inCombat)
            AudioManager.PlayOneShot(eEffects.farmWater, src, 100);
        else if (spellType == SpellType.Air && !inCombat)
            AudioManager.PlayOneShot(eEffects.farmAir, src, 100);
    }
}
