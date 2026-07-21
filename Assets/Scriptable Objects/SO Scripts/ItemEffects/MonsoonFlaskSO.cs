using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Monsoon", menuName = "Scriptable Objects/ItemEffects/MonsoonFlaskSO")]
public class MonsoonFlaskSO : ItemEffectSO
{
    [Header("Prefab Settings")]
    public GameObject projectilePrefab;
    
    public override void UseItem()
    {
        sfx = GameObject.FindWithTag("ItemSFX").GetComponent<ItemSFXManager>();

        Transform playerTransform = ThirdPersonController.Instance.transform;
        Activate(playerTransform);

        sfx.PlayOneShotForItem(eEffects.potion);
        sfx.PlayOneShotForItem(eEffects.farmWater);
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
