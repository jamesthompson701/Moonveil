using UnityEngine;

public class ProximityGlow : MonoBehaviour
{
    public Light glowLight;
    public ParticleSystem glowParticles;

    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip lightSound;
    public AudioClip extinguishSound;

    void Start()
    {
        if (glowLight != null)
        {
            glowLight.enabled = false;
        }

        if (glowParticles != null)
        {
            glowParticles.Stop();
        }

        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger");
        if (other.CompareTag("Player"))
        {
            if (glowLight != null)
            {
                glowLight.enabled = true;
            }

            if (glowParticles != null)
            {
                glowParticles.Play();
            }

            audioSource.PlayOneShot(lightSound);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (glowLight != null)
            {
                glowLight.enabled = false;
            }

            if (glowParticles != null)
            {
                glowParticles.Stop();
            }

            audioSource.PlayOneShot(extinguishSound);
        }
    }
}