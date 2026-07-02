using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    public PengKingBoss PengKingBoss;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.CompareTag("Player"))
        {
            if (PengKingBoss != null)
                PengKingBoss.ActivateShieldAndRandomWeakpoints();
            else
                Debug.LogWarning("BossFightTrigger: PengKingBoss reference not assigned.");
        }
    }
}
