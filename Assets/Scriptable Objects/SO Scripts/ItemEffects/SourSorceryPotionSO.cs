using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SourSorceryPotion", menuName = "Scriptable Objects/ItemEffects/SourSorceryPotion")]
public class SourSorceryPotionSO : ItemEffectSO
{
    // attributes
    public float buffLength;

    public override void UseItem()
    {
        sfx = GameObject.FindWithTag("ItemSFX").GetComponent<ItemSFXManager>();

        EnemyAttackDirector.Instance.sourSorceryTimer = buffLength;
        EnemyAttackDirector.Instance.isSourSorceryActive = true;

        sfx.PlayOneShotForItem(eEffects.potion);
    }

}
