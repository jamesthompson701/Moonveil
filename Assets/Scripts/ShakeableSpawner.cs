using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShakeableSpawner : MonoBehaviour
{
    //things to spawn (chosen from at random)
    public List<GameObject> drops;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HarvestSpell") || other.CompareTag("AirAttack"))
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Instantiate(drops[Random.Range(0, drops.Count)], spawnPos, transform.rotation);
            //SpawnFromTable();
            Debug.Log("spawned?");
        }
    }

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

}
