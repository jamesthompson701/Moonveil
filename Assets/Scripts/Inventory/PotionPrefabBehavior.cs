using StarterAssets;
using UnityEngine;

public class PotionPrefabBehavior : MonoBehaviour
{
    public bool followPlayer = false;
    public float destroyTime = 5;
    public float followSpeed = 5f;

    private Transform playerTransform;

    private void Start()
    {
        if (followPlayer)
        {
            playerTransform = ThirdPersonController.Instance.transform;
            if (playerTransform == null)
            {
                Debug.LogWarning("PotionPrefabBehavior: Could not find player transform.");
            }
        }

        // Destroy the potion prefab after 5 seconds
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        if (followPlayer && playerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
        }
    }
}
