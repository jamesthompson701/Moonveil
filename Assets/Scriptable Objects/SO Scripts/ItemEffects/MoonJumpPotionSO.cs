using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MoonJumpPotion", menuName = "Scriptable Objects/ItemEffects/MoonJumpPotion")]
public class MoonJumpPotionSO : ItemEffectSO
{
    //attributes
    public float newJumpHeight;
    public float newGravity;
    public float buffLength;

    public override void UseItem()
    {
        ThirdPersonController.Instance.moonJumpTimer = buffLength;
        ThirdPersonController.Instance.isMoonJumpActive = true;
        ThirdPersonController.Instance.JumpHeight = newJumpHeight;
        ThirdPersonController.Instance.Gravity = newGravity;
    }
}
