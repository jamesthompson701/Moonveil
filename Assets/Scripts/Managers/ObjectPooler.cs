using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPool", menuName = "Scriptable Objects/ObjectPool")]
public class ObjectPool : ScriptableObject
{
    public GameObject objectToPool;
    public int amountToPool;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    //This is an array of the different pools that we will have in the game. Its an array of SOs and this is how we tell the manager what we want pools of
    public ObjectPool[] poolsArray;

    private Dictionary<GameObject, List<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<GameObject, List<GameObject>>();

        foreach (ObjectPool pool in poolsArray)
        {
            //TODO: make a parent for each pool
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.amountToPool; i++)
            {
                GameObject obj = Instantiate(pool.objectToPool, transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.objectToPool, objectPool);

        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        foreach (GameObject obj in poolDictionary[prefab])
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        return null; // or expand pool here if you want
    }
}