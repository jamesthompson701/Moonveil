using System.Collections;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Health")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    [SerializeField, Tooltip("Shown for debugging; update your UI as needed.")]
    private float currentHealth = 100f;

    [SerializeField, Tooltip("Invincibility duration in seconds after taking damage.")]
    private float invincibilityDuration = 2;

    // Renderers and materials set in the inspector (kept original names to preserve serialized references)
    public GameObject playerBodyDefault;
    public Material playerDamaged;
    public Material PlayerDefault;

    // Internal caches
    private Material _bodyOriginalMaterial;
    private float _invincibilityTimer = 0f;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Cache the original materials from the renderers if possible.
        // Prefer the renderer's shared material (asset) so we restore exactly what was assigned in the scene.
        if (playerBodyDefault != null)
        {
            if (_bodyOriginalMaterial == null && PlayerDefault != null)
                _bodyOriginalMaterial = PlayerDefault;
        }
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
        {
            _invincibilityTimer -= Time.deltaTime;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void CheckInvincibleFrames(float amount)
    {
        if (_invincibilityTimer > 0f) return; // Player is currently invincible
        TakeDamage(amount);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        // If already invincible, ignore additional damage
        if (_invincibilityTimer > 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log("You took " + amount + " damage! Current health: " + currentHealth);

        // Start invincibility and visual feedback
        _invincibilityTimer = invincibilityDuration;

        StartCoroutine(ShowDamageTaken());
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Implement death logic (e.g., respawn, game over screen)
    }

    private IEnumerator ShowDamageTaken()
    {
        if (playerBodyDefault != null && playerDamaged != null)
        {
            for (int i = 0; i < playerBodyDefault.transform.childCount; i++)
            {
                var child = playerBodyDefault.transform.GetChild(i);
                var renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = playerDamaged;
                }
            }
        }
        else if (playerBodyDefault != null && playerDamaged == null)
        {
            Debug.LogWarning("playerDamaged is not assigned; skipping body material swap.");
        }

        // Wait for the configured invincibility duration (visual feedback time)
        yield return new WaitForSeconds(invincibilityDuration);

        // Restore original materials
        RestoreOriginalMaterials();
    }

    private void RestoreOriginalMaterials()
    {
        if (playerBodyDefault != null)
        {
            for (int i = 0; i < playerBodyDefault.transform.childCount; i++)
            {
                var child = playerBodyDefault.transform.GetChild(i);
                var renderer = child.GetComponent<Renderer>();
                if (renderer != null && _bodyOriginalMaterial != null)
                {
                    renderer.material = _bodyOriginalMaterial;
                }
            }
        }
    }
}