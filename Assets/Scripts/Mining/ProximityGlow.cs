using UnityEngine;

public class ProximityGlow : MonoBehaviour
{
    public Light glowLight;
    public ParticleSystem glowParticles;
    public bool alwaysOn = false;

    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip lightSound;
    public AudioClip idleSound;
    public AudioClip extinguishSound;

    void Start()
    {
        if (alwaysOn)
        {
            if (glowLight != null)
                glowLight.enabled = true;

            if (glowParticles != null)
                glowParticles.Play();

            if (audioSource != null && idleSound != null)
            {
                audioSource.clip = idleSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            return;
        }


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

            if (audioSource != null)
            {
                audioSource.PlayOneShot(lightSound);

                audioSource.clip = idleSound;
                audioSource.loop = true;
                audioSource.Play();
            }
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

            audioSource.Stop();
            audioSource.PlayOneShot(extinguishSound);
        }
    }
}