using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spells", menuName = "Scriptable Objects/SO_Spells")]
public abstract class SO_Spells : ScriptableObject
{
    [Header("Info")]
    public string SpellName;

    [Header("Prefab")]
    public Rigidbody SpellPrefab;

    [Header("Stats")]
    public int Damage;
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

/// <summary>
/// Attack spells for each of the four elements
/// </summary>

//[CreateAssetMenu(fileName = "FireAttackSpells", menuName = "Scriptable Objects/FireAttackSpells")]
//public class FireAttackSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        SpellPrefab = Instantiate(SpellPrefab, castOrigin.position + castOrigin.forward * spawnOffset, castOrigin.rotation);
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "WaterAttackSpells", menuName = "Scriptable Objects/WaterAttackSpells")]
//public class WaterAttackSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        SpellPrefab = Instantiate(SpellPrefab, castOrigin.position + castOrigin.forward * spawnOffset, castOrigin.rotation);
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "AirAttackSpells", menuName = "Scriptable Objects/AirAttackSpells")]
//public class AirAttackSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        SpellPrefab = Instantiate(SpellPrefab, castOrigin.position + castOrigin.forward * spawnOffset, castOrigin.rotation);
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "EarthAttackSpells", menuName = "Scriptable Objects/EarthAttackSpells")]
//public class EarthAttackSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        SpellPrefab = Instantiate(SpellPrefab, castOrigin.position + castOrigin.forward * spawnOffset, castOrigin.rotation);
//        throw new System.NotImplementedException();
//    }
//}
//
///// <summary>
///// Farming spells for each of the four elements
///// </summary>
//
//[CreateAssetMenu(fileName = "FireFarmSpells", menuName = "Scriptable Objects/FireFarmSpells")]
//public class FireFarmSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "WaterFarmSpells", menuName = "Scriptable Objects/WaterFarmSpells")]
//public class WaterFarmSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "AirFarmSpells", menuName = "Scriptable Objects/AirFarmSpells")]
//public class AirFarmSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        throw new System.NotImplementedException();
//    }
//}
//
//[CreateAssetMenu(fileName = "EarthFarmSpells", menuName = "Scriptable Objects/EarthFarmSpells")]
//public class EarthFarmSpells : SO_Spells
//{
//    public override void CastSpell(Transform castOrigin, Camera aimCamera, LayerMask aimMask, float aimDistance)
//    {
//        throw new System.NotImplementedException();
//    }
//}
