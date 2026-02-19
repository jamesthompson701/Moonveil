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
    //name of the plant
    public string plantName;

    //how long the plant takes to increment 1 stage
    public float cropTime;

    //how long the plant can be dry for before dying
    public float droughtResistance;

    //item IDs of the seed, fruit, and a placeholder if we need it
    public int seedID;
    public int fruitID;
    public int secretThirdThingID;

    //prefabs for each growth stage of the plant
    public List<GameObject> plantPrefabs;

    //prefab for the plant object
    public GameObject prefab;

    //prefab for when the plant is dead
    public GameObject plantDead;

    //used for harvestability
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
