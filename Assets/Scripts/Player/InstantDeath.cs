using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (PlayerDamageReceiver.instance != null)
        {
            PlayerDamageReceiver.instance.currentHealth = 0f;
        }

        //Deal damage for lava damage ticks, around 15% per?
    }
}