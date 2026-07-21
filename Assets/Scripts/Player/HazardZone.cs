using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [Header("Damage")]
    public bool instantKill = true;

    public float damagePerTick = 15f;
    public float tickInterval = 1f;

    float timer;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (PlayerDamageReceiver.instance == null)
        {
            return;
        }

        if (instantKill)
        {
            PlayerDamageReceiver.instance.currentHealth = 0;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            PlayerDamageReceiver.instance.TakeDamage(damagePerTick);
            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}
