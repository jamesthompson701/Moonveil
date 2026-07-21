using UnityEngine;

public class AudioZone : MonoBehaviour
{
    [Header("Select Music for this Terrain Area")]
    [SerializeField] private eMusic terrainMusic;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            // Call your static method directly via the Singleton Instance
            AudioManager.ChangeTrack(terrainMusic);
        }
    }
}
