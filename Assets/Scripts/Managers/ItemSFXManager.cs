using UnityEngine;

public class ItemSFXManager : MonoBehaviour
{
    public GameObject player;
    public ItemSFXManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        player = GameObject.FindWithTag("Player");
    }

    public void PlayOneShotForItem(eEffects _sound)
    {
        AudioManager.PlayOneShot(_sound, player.transform, 100);
    }
}
