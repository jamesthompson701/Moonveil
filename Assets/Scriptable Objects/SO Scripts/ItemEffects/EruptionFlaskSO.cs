using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Eruption", menuName = "Scriptable Objects/ItemEffects/EruptionFlaskSO")]
public class EruptionFlaskSO : ItemEffectSO
{
    [Header("Prefab Settings")]
    public GameObject projectilePrefab;

    public override void UseItem()
    {
        sfx = GameObject.FindWithTag("ItemSFX").GetComponent<ItemSFXManager>();

        Transform playerTransform = ThirdPersonController.Instance.transform;
        Activate(playerTransform);

        sfx.PlayOneShotForItem(eEffects.potion);
        sfx.PlayOneShotForItem(eEffects.farmFire);
        sfx.PlayOneShotForItem(eEffects.farmEarth);
    }

    private void Activate(Transform playerTransform)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("EruptionFlaskSO: projectilePrefab is not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("EruptionFlaskSO: playerTransform is null.");
            return;
        }

        Instantiate(projectilePrefab, playerTransform.position, Quaternion.identity);
    }
}
