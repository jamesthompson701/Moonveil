using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Cotton Spore", menuName = "Scriptable Objects/ItemEffects/CottonSportPotionSO")]
public class CottonSporePotionSO : ItemEffectSO
{
    [Header("Prefab & Launch Settings")]
    public GameObject projectilePrefab;
    public int projectileCount = 5;
    
    [Header("Launch Force")]
    public float launchForce = 10f;

    [Header("Spawn Offset")]
    public float verticalOffset = 1.5f;
    
    public override void UseItem()
    {
        Transform playerTransform = ThirdPersonController.Instance.transform;
        Activate(playerTransform);
    }

    private void Activate(Transform playerTransform)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("CottonSporePotionSO: projectilePrefab is not assigned.");
            return;
        }

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 spawnPosition = playerTransform.position + Vector3.up * verticalOffset;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            Vector3 randomDirection = Random.onUnitSphere;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = randomDirection * launchForce;
            }
        }
    }
}
