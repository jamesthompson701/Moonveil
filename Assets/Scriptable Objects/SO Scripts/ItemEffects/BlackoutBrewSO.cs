using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BlackoutBrew", menuName = "Scriptable Objects/ItemEffects/BlackoutBrew")]
public class BlackoutBrewSO : ItemEffectSO
{

    public override void UseItem()
    {
        TimeManager.instance.Sleep();
        TimeManager.instance.isBlackout = true;
        TimeManager.instance.blackoutTimer = 5f;
    }
}
