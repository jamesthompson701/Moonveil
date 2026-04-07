using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShakeableSpawner : MonoBehaviour
{
    //things to spawn (chosen from at random)
    public List<GameObject> drops;
    private GameObject droppedItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HarvestSpell") || other.CompareTag("AirAttack"))
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Instantiate(drops[Random.Range(0, drops.Count)], spawnPos, transform.rotation);
            Debug.Log("spawned?");
        }
    }

}
