using UnityEngine;

public class AudioZone : MonoBehaviour
{
    [Header("Zone Music Configuration")]
    [Tooltip("Select which music track belongs to this specific terrain collider.")]
    [SerializeField] private eMusic zoneMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager the player entered this specific zone
            AudioManager.UpdateZoneMusic(zoneMusic, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager the player left this specific zone
            AudioManager.UpdateZoneMusic(zoneMusic, false);
        }
    }
}
