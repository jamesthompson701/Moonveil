using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MoonChildSO(unused)", menuName = "Scriptable Objects/ItemEffects/MoonChildSO")]
public class MoonChild : ItemEffectSO
{
    //need these to restore the original speed
    public float originalSpeed;
    public float originalHeight;

    public float newMoveSpeed;
    public float newJumpHeight;
    public float buffLength;

    public override void UseItem()
    {
        ThirdPersonController.Instance.MoveSpeed = newMoveSpeed;
        // times 2.65 to preserve the same ratio that the defaults have to eachother
        ThirdPersonController.Instance.SprintSpeed = newMoveSpeed * 2.65f;
        ThirdPersonController.Instance.JumpHeight = newJumpHeight;
        //ThirdPersonController.Instance.MoonBuff(buffLength);
    }
}