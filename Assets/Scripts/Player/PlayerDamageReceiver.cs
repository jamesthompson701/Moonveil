using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    [SerializeField, Tooltip("Shown for debugging; update your UI as needed.")]
    private float currentHealth = 100f;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, transform.position, Vector3.zero, 0f, null);
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection, float impulseForce, GameObject instigator)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log("You took " + amount + " damage! Current health: " + currentHealth);
    }
}
