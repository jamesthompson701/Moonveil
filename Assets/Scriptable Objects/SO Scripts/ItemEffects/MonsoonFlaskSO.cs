using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Monsoon", menuName = "Scriptable Objects/ItemEffects/MonsoonFlaskSO")]
public class MonsoonFlaskSO : ItemEffectSO
{
    [Header("Prefab Settings")]
    public GameObject projectilePrefab;
    
    public override void UseItem()
    {
        Transform playerTransform = ThirdPersonController.Instance.transform;
        Activate(playerTransform);
    }

    private void Activate(Transform playerTransform)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("MonsoonFlaskSO: projectilePrefab is not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("MonsoonFlaskSO: playerTransform is null.");
            return;
        }

        Instantiate(projectilePrefab, playerTransform.position, Quaternion.identity);
    }
}
