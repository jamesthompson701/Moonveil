using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "MoonChildSO", menuName = "Scriptable Objects/ItemEffects/MoonChildSO")]
public class MoonChild : ItemEffectSO
{
    public float newMoveSpeed;
    public float newJumpHeight;
    public float buffLength;

    public override void UseItem()
    {
        ThirdPersonController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        playerController.MoveSpeed = newMoveSpeed;
        playerController.JumpHeight = newJumpHeight;
    }
}