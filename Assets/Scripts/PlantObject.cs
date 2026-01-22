using System;
using UnityEngine;

//Plants can grow
//Tracks its own current growth stage, how long since it last grew, and its current object in the world
public class PlantObject : MonoBehaviour
{
    [SerializeField] private PlantSO plant;

    private int currentStage;
    private float currentTime;
    private GameObject currentPlant;

    void Start()
    {
        //add to time manager and instantiate the first prefab
        TimeManager.instance.RegisterPlant(this);
        currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
    }

    public void CheckPlant(float deltaTime)
    {
        //update currentTime
        currentTime = currentTime + deltaTime;

        //if it's time to grow, reset timer
        if (currentTime >= plant.CropTime)
        {
            currentTime = 0;

            //then if it's at max size, don't increment currentStage and debug log
            if (currentStage >= plant.MaxStage)
            {
                Debug.Log("Harvestable");
                currentStage = plant.MaxStage;
            }
            //if it's not at max, increment currentStage
            else
            {
                currentStage++;
                if (currentStage >= plant.MaxStage)
                {
                    Debug.Log("Harvestable");
                }
            }
            
            Debug.Log(currentStage);
            
            //if a prefab exists for the current stage,
            //destroy the current object and make a new one at the new growth stage
            if (plant.GetPrefabByStage(currentStage) != null)
            {
                Destroy(currentPlant);
                currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            }
        }
    }

    //returns true if the plant is at max growth
    public bool HasMaxLevel()
    {
        return currentStage == plant.MaxStage;
    }
}
