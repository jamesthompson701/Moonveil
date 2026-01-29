using System;
using UnityEngine;

//Plants can grow
//Tracks its own current growth stage, how long since it last grew, and its current object in the world
public class PlantObject : MonoBehaviour
{
    [SerializeField] private PlantSO plant;

    //current stage of growth
    private int currentStage;
    //how long since it last grew
    private float currentTime;

    //harvestability
    private bool isHarvestable;

    //plant object
    private GameObject currentPlant;
    private SoilObject soilScript;

    void Start()
    {
        //add to time manager and instantiate the first prefab
        currentStage = 0;
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
            Debug.Log("before growth: " + currentStage);
            currentTime = 0;

            //then increment, but not past the max
            if (currentStage < plant.MaxStage)
            {
                currentStage++;
            }
            if (currentStage == plant.MaxStage)
            {
                isHarvestable = true;
                Debug.Log("Harvestable!");
            }
            
            //if a prefab exists for the current stage,
            //destroy the current object and make a new one at the new growth stage
            if (plant.GetPrefabByStage(currentStage) != null)
            {
                Destroy(currentPlant);
                currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            }
            Debug.Log("after growth: " + currentStage);
        }
        
    }

    //returns true if the plant is at max growth a.k.a. harvestable
    public bool Harvestable()
    {
        return isHarvestable;
    }

    public void Harvest()
    {
        PlayerInventory.instance.AddSeeds(2);
        Debug.Log("Harvested");
        Destroy(currentPlant);
        TimeManager.instance.UnregisterPlant(this);
        Destroy(this);
    }

    public void Destroy()
    {
        Debug.Log("Destroyed");
        Destroy(currentPlant);
        TimeManager.instance.UnregisterPlant(this);
        Destroy(this);
    }

    //function to be called by SoilObject 
    public void SetSoil(SoilObject _soil)
    {
        soilScript = _soil;
    }
}
