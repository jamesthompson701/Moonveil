using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [Header("Damage")]
    public bool instantKill = true;

    public float damagePerTick = 15f;
    public float tickInterval = 1f;

    [Header("Special Hazards")]
    public bool instantHit = false;

    float timer;
    bool hasHit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (PlayerDamageReceiver.instance == null)
        {
            return;
        }

        // For single-hit hazards like stalactites
        if (instantHit)
        {
            if (hasHit)
            {
                return;
            }

            hasHit = true;

            if (instantKill)
            {
                PlayerDamageReceiver.instance.currentHealth = 0;
            }
            else
            {
                PlayerDamageReceiver.instance.TakeDamage(damagePerTick);
            }

            return;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (instantHit)
        {
            return;
        }

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
            hasHit = false;
        }
    }
}