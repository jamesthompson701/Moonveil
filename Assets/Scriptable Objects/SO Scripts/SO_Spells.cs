using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spells", menuName = "Scriptable Objects/SO_Spells")]
public abstract class SO_Spells : ScriptableObject
{
    [Header("Info")]
    public string SpellName;

    [Header("Prefab")]
    public Rigidbody SpellPrefab;

    [Header("Stats")]
    public int damage;
    public float Speed = 15f;

    [Header("Lifetime")]
    public float Lifetime = 3f;

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
