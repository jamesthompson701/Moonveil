using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//SO for a plant
//Has a name, how long it takes to increment growth, its maximum stage,
//and a list of prefabs for its different growth stages
//Also has a function to get the correct prefab using the stage number
[CreateAssetMenu(fileName = "PlantSO", menuName = "Scriptable Objects/PlantSO")]
public class PlantSO : ScriptableObject
{
    public string plantName;
    public float CropTime;
    public List<GameObject> plantPrefabs;
    public GameObject seedPrefab;

    public int MaxStage { get { return plantPrefabs.Count - 1; } }

    //returns the correct prefab for the growth stage inputted
    public GameObject GetPrefabByStage(int stage)
    {
        if (stage >= MaxStage + 1)
        {
            return null;
        }
        return plantPrefabs[stage];
    }
}
