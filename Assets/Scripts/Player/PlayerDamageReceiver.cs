using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Health")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    [SerializeField, Tooltip("Shown for debugging; update your UI as needed.")]
    private float currentHealth = 100f;

    [SerializeField, Tooltip("Add invincibility frames after taking damage.")]
    private float invincibilityDuration = 0f;

    private SceneManager sceneManager;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Update()
    {
        if (invincibilityDuration > 0f)
        {
            invincibilityDuration -= Time.deltaTime;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        if (invincibilityDuration > 0f) return; // Player is currently invincible
        TakeDamage(amount, transform.position, Vector3.zero, null);
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection, GameObject instigator)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log("You took " + amount + " damage! Current health: " + currentHealth);

        // handles hit visual effect by applying a red flash to the player model for a brief moment then flashing white while invincible
        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)
        {
            Renderer renderer = GetComponentsInChildren<Renderer>()[i];
            if (renderer != null)
            {
                if (invincibilityDuration > 0f)
                {
                    renderer.material.color = Color.white; // Flash white while invincible
                }
                else
                {
                    renderer.material.color = Color.red; // Flash red when hit
                    Invoke("ResetColor", 0.1f); // Reset color after a short delay
                }
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        sceneManager.LoadSceneTest();
        // Implement death logic (e.g., respawn, game over screen)
    }

    private void ResetColor()
    {
        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)
        {
            Renderer renderer = GetComponentsInChildren<Renderer>()[i];
            if (renderer != null)
            {
                renderer.material.color = Color.grey; // Reset to original color
            }
        }
    }
}
