using System;
using UnityEngine;

//Plants can grow
//Tracks its own current growth stage, how long since it last grew, and its current object in the world
public class PlantObject : MonoBehaviour
{
    [SerializeField] private PlantSO plant;

    private int currentStage;
    //how long since it last grew
    private float currentTime;
    //plant object
    private GameObject currentPlant;
    private GameObject mySoil;

    void Start()
    {
        //add to time manager and instantiate the first prefab
        TimeManager.instance.RegisterPlant(this);
        currentPlant = Instantiate(plant.GetPrefabByStage(0), transform);
    }

    public void CheckPlant(float deltaTime)
    {
        //update currentTime
        currentTime = currentTime + deltaTime;

        //if it's time to grow, reset timer
        if (currentTime >= plant.CropTime)
        {
            currentTime = 0;

            //then increment, but not past the max
            if (currentStage < plant.MaxStage)
            {
                currentStage++;
            }
            else
            {
                currentStage = plant.MaxStage;
            }
            
            Debug.Log(currentStage);
            if (currentStage == plant.MaxStage)
            {
                Debug.Log("Harvestable!");
            }
            
            //if a prefab exists for the current stage,
            //destroy the current object and make a new one at the new growth stage
            if (plant.GetPrefabByStage(currentStage) != null)
            {
                Destroy(currentPlant);
                currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            }
        }
    }

    //returns true if the plant is at max growth a.k.a. harvestable
    public bool Harvestable()
    {
        return currentStage == plant.MaxStage;
    }

    public void Harvest()
    {
        Debug.Log("Harvested");

        if (plant.GetPrefabByStage(currentStage) != null)
        {
            
            /*Debug.Log("pre-harvest: " + currentStage);
            currentStage = plant.MaxStage - 1;
            Debug.Log("post-harvest: " + currentStage);*/

            Destroy(currentPlant);
            TimeManager.instance.UnregisterPlant(this);
            Destroy(this);

        }
    }
}
