using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShakeableSpawner : MonoBehaviour
{
    //seed to spawn + particle
    public GameObject seed;
    public GameObject shakenParticle;

    //how many seeds are currently available to spawn
    private int currentSeeds;

    private void Start()
    {
        currentSeeds = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HarvestSpell") || other.CompareTag("AirAttack"))
        {
            if (currentSeeds > 0)
            {
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                GameObject newSeed = Instantiate(seed, spawnPos, transform.rotation);
                Rigidbody newSeedRB = newSeed.GetComponent<Rigidbody>();

                int randomNum1 = Random.Range(-4, 5);
                int randomNum2 = Random.Range(4, 9);
                int randomNum3 = Random.Range(-4, 5);
                newSeedRB.linearVelocity = new Vector3(randomNum1, randomNum2, randomNum3);

                Instantiate(shakenParticle, spawnPos, transform.rotation);
                currentSeeds = currentSeeds - 1;
                Invoke("ReplenishSeed", 60f);
            }
        }
    }

    private void ReplenishSeed()
    {
        currentSeeds++;
    }

}
