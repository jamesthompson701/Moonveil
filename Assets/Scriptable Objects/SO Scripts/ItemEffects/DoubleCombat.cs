using UnityEngine;

[CreateAssetMenu(fileName = "DoubleCombatSO", menuName = "Scriptable Objects/ItemEffects/DoubleCombatSO")]
public class DoubleCombat : ItemEffectSO
{
    public int dropMultiplier;
    public float buffLength;

    public override void UseItem()
    {
        InventoryManager.instance.invSO.combatMultiplier = dropMultiplier;
        InventoryManager.instance.combatBuffTime = buffLength;
        InventoryManager.instance.isCombatBuffActive = true;
    }
}