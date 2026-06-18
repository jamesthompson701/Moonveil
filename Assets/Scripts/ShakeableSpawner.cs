using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShakeableSpawner : MonoBehaviour
{
    //things to spawn (chosen from at random)
    //public List<GameObject> drops;

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
                int randomNum1 = Random.Range(-1, 2);
                int randomNum2 = Random.Range(-1, 2);
                int randomNum3 = Random.Range(-1, 2);
                Vector3 spawnPos = new Vector3(transform.position.x + randomNum1, transform.position.y + randomNum2, transform.position.z + randomNum3);
                Instantiate(seed, spawnPos, transform.rotation);
                Instantiate(shakenParticle, spawnPos, transform.rotation);
                currentSeeds = currentSeeds - 1;
                Invoke("ReplenishSeed", 60f);
                //SpawnFromTable();
                //Debug.Log("spawned seed");
            }
        }
    }

    private void ReplenishSeed()
    {
        currentSeeds++;
    }

    /*
    private void SpawnFromTable()
    {
        GameObject obj = ObjectPooler.Instance.GetPooledObject(drops[Random.Range(0, drops.Count)]);
        if (obj != null)
        {
            obj.transform.position = transform.position + Vector3.up * 2;
            obj.transform.rotation = transform.rotation;
            obj.SetActive(true);
        }
    }
    */

}
