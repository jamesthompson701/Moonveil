using UnityEngine;

[CreateAssetMenu(fileName = "DoubleMiningSO", menuName = "Scriptable Objects/ItemEffects/DoubleMiningSO")]
public class DoubleMining : ItemEffectSO
{
    public int dropMultiplier;
    public float buffLength;

    public override void UseItem()
    {
        InventoryManager.instance.invSO.miningMultiplier = dropMultiplier;
        InventoryManager.instance.miningBuffTime = buffLength;
        InventoryManager.instance.isMiningBuffActive = true;
    }
}