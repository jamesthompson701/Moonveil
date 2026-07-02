using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LifeForcePotion", menuName = "Scriptable Objects/ItemEffects/LifeForcePotion")]
public class LifeForcePotionSO : ItemEffectSO
{
    public override void UseItem()
    {
        PlayerDamageReceiver.instance.currentHealth += 25;
        if(PlayerDamageReceiver.instance.currentHealth > PlayerDamageReceiver.instance.maxHealth)
        {
            PlayerDamageReceiver.instance.currentHealth = PlayerDamageReceiver.instance.maxHealth;
        }
    }
}
