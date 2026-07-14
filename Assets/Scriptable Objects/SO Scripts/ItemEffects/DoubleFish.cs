using UnityEngine;

[CreateAssetMenu(fileName = "DoubleFishSO", menuName = "Scriptable Objects/ItemEffects/DoubleFishSO")]
public class DoubleFish : ItemEffectSO
{
    public int dropMultiplier;
    public float buffLength;

    public override void UseItem()
    {
        InventoryManager.instance.invSO.dropMultiplier = dropMultiplier;
        InventoryManager.instance.multiplierBuffTime = buffLength;
        InventoryManager.instance.isMultiplierBuffActive = true;
    }
}