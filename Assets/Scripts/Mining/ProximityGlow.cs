using UnityEngine;

public class ProximityGlow : MonoBehaviour
{
    public Light glowLight;
    public ParticleSystem glowParticles;

    void Start()
    {
        if (glowLight != null)
            glowLight.enabled = false;

        if (glowParticles != null)
            glowParticles.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger");
        if (other.CompareTag("Player"))
        {
            if (glowLight != null)
                glowLight.enabled = true;

            if (glowParticles != null)
                glowParticles.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (glowLight != null)
                glowLight.enabled = false;

            if (glowParticles != null)
                glowParticles.Stop();
        }
    }
}