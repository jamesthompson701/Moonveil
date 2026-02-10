using System;
using UnityEngine;
using TMPro;

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

    //plant object & soil
    private GameObject currentPlant;
    private SoilObject soilScript;

    //canvas and growth timer
    public Canvas myCanvas;
    public TMP_Text growthTimer;

    void Awake()
    {
        //add to time manager and instantiate the first prefab
        currentStage = 0;
        TimeManager.instance.RegisterPlant(this);
        currentTime = plant.CropTime;
        currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
    }

    public void CheckPlant(float deltaTime)
    {
        
        //update currentTime
        if(currentTime > 0)
        {
            currentTime = currentTime - deltaTime;
        }
        else
        {
            //check wetness before growing
            if (soilScript.Wet())
            {
                Debug.Log("before growth: " + currentStage);
                currentTime = plant.CropTime;

                //then increment, but not past the max
                if (currentStage < plant.MaxStage)
                {
                    currentStage++;
                }
                if (currentStage == plant.MaxStage)
                {
                    isHarvestable = true;
                    Destroy(growthTimer);
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

        //update growth timer UI
        growthTimer.text = "" + Mathf.Round(currentTime);

        
        
        
        
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
        Destroy(myCanvas);
        Destroy(this);
    }

    public void Destroy()
    {
        Debug.Log("Destroyed");
        Destroy(currentPlant);
        TimeManager.instance.UnregisterPlant(this);
        Destroy(myCanvas);
        Destroy(this);
    }

    //function to be called by SoilObject 
    public void SetSoil(SoilObject _soil)
    {
        soilScript = _soil;
    }
}
