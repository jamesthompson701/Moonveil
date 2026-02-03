using UnityEngine;

public class SpellParticles : MonoBehaviour
{
    public GameObject waterParticle;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Water Particle Spawned");
        Instantiate(waterParticle, transform);
    }
}
