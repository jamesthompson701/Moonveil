using UnityEngine;

public class AudioZone : MonoBehaviour
{
    [Header("Zone Music Settings")]
    [SerializeField] private eMusic zoneMusicTrack;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the player
        if (other.CompareTag("Player"))
        {
            // Tells AudioManager we entered this zone (handles fading in)
            AudioManager.UpdateZoneMusic(zoneMusicTrack, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the zone is the player
        if (other.CompareTag("Player"))
        {
            // Tells AudioManager we left this zone (handles fading back to previous music)
            AudioManager.UpdateZoneMusic(zoneMusicTrack, false);
        }
    }
}
