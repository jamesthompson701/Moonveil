using UnityEngine;

[CreateAssetMenu(fileName = "DoubleFishSO", menuName = "Scriptable Objects/ItemEffects/DoubleFishSO")]
public class DoubleFish : ItemEffectSO
{
    public int dropMultiplier;
    public float buffLength;

    public override void UseItem()
    {
        InventoryManager.instance.invSO.fishMultiplier = dropMultiplier;
        InventoryManager.instance.fishBuffTime = buffLength;
        InventoryManager.instance.isFishBuffActive = true;
    }
}