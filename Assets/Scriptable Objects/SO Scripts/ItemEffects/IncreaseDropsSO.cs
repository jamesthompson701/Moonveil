using UnityEngine;

[CreateAssetMenu(fileName = "DropMultiplierSO", menuName = "Scriptable Objects/ItemEffects/DropMultiplierSO")]
public class DropMultiplier : ItemEffectSO
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