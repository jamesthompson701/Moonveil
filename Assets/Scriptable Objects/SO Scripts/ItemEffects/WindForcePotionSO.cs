using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WindForcePotion", menuName = "Scriptable Objects/ItemEffects/WindForcePotion")]
public class WindForcePotionSO : ItemEffectSO
{
    //attributes
    public float newMoveSpeed;
    public float newSprintSpeed;
    public float newFlightMoveSpeed;
    public float newFlightDeceleration;
    public float buffLength;

    public override void UseItem()
    {
        sfx = GameObject.FindWithTag("ItemSFX").GetComponent<ItemSFXManager>();

        ThirdPersonController.Instance.windForceTimer = buffLength;
        ThirdPersonController.Instance.isWindForceActive = true;
        ThirdPersonController.Instance.MoveSpeed = newMoveSpeed;
        ThirdPersonController.Instance.SprintSpeed = newSprintSpeed;
        ThirdPersonController.Instance.FlightMoveSpeed = newFlightMoveSpeed;
        ThirdPersonController.Instance.FlightVelocityDeceleration = newFlightDeceleration;

        sfx.PlayOneShotForItem(eEffects.potion);
    }
}
