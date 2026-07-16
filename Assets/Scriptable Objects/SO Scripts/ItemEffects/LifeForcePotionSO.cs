using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LifeForcePotion", menuName = "Scriptable Objects/ItemEffects/LifeForcePotion")]
public class LifeForcePotionSO : ItemEffectSO
{
    public override void UseItem()
    {
        sfx = GameObject.FindWithTag("ItemSFX").GetComponent<ItemSFXManager>();

        PlayerDamageReceiver.instance.currentHealth += 25;
        if(PlayerDamageReceiver.instance.currentHealth > PlayerDamageReceiver.instance.maxHealth)
        {
            PlayerDamageReceiver.instance.currentHealth = PlayerDamageReceiver.instance.maxHealth;
        }

        sfx.PlayOneShotForItem(eEffects.potion);
    }
}
