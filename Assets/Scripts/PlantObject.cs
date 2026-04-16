using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Tracks its own current growth stage, how long since it last grew, how long its been dry and its current object in the world
public class PlantObject : MonoBehaviour
{
    [SerializeField] public PlantSO plant;

    //current stage of growth
    private int currentStage;

    //timers
    //these get set to their respective maximums based on plantSO, and then count down as appropriate via CheckPlant
    private float growthTime;
    private float dryTime;

    //harvestability and withered status
    private bool isHarvestable;
    private bool withered;

    //plant object & soil
    private GameObject currentPlant;
    private SoilObject soilScript;

    //canvas and growth timer
    public Canvas myCanvas;
    public TMP_Text growthTimer;
    public Image growthProgressBar;

    public TMP_Text waterTimer;
    public Image waterTimerBar;

    //bool to toggle if it's been setup
    private bool isSet;

    void Awake()
    {
        //add to time manager
        currentStage = 0;
        TimeManager.instance.RegisterPlant(this);

    }

    //checkplant
    // light refers to the time of day; 1 = morning, 2 = evening, 3 = night
    public void CheckPlant(float deltaTime, int _light)
    {
        if (!isSet)
        {
            //set the plant SO correctly based on the seed used
            growthTime = plant.cropTime;
            dryTime = plant.droughtResistance;
            currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            isSet = true;

            //tutorial
            if (TutorialManager.instance != null && !TutorialManager.instance.planting)
            {
                //completes billboard 3: plant seeds
                TutorialManager.instance.ProgressTutorial(3);
                TutorialManager.instance.planting = true;
            }
        }

        //increment the dry timer while dry
        if(!soilScript.Wet())
        {

            //if it's been dry too long, it withers
            if(dryTime < plant.droughtResistance / 2)
            {
                Wither();
            }

        }

        //update growth time as long as the plant isn't withered, the light is appropriate, and it isn't harvestable
        if (growthTime > 0 && soilScript.Wet() && !withered && plant.lightPreference == _light && !isHarvestable)
        {
            growthTime = growthTime - deltaTime;
            if (withered)
            {
                //if plant is growing that means it's time to unwither it
                Unwither();
            }

        }
        else
        {
            //check wetness again before growing
            if (soilScript.Wet())
            {
                Debug.Log("before growth: " + currentStage);

                //reset growth timer
                growthTime = plant.cropTime;

                //then increment, but not past the max
                if (currentStage < plant.MaxStage)
                {
                    currentStage++;
                    
                }
                if (currentStage == plant.MaxStage)
                {
                    isHarvestable = true;
                    Destroy(myCanvas);
                    Debug.Log("Harvestable!");
                }

                //destroy the current object and make a new one at the new growth stage
                if (plant.GetPrefabByStage(currentStage) != null)
                {
                    Destroy(currentPlant);
                    currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
                }
                Debug.Log("after growth: " + currentStage);
            }
        }

        //update growth timer UI and water timer UI (skip if harvestable)
        if (!isHarvestable)
        {
            growthTimer.text = "" + Mathf.Round(growthTime);
            growthProgressBar.fillAmount = growthTime / plant.cropTime;

            waterTimer.text = " " + Mathf.Round(soilScript.waterTimer);
            waterTimerBar.fillAmount = soilScript.waterTimer / soilScript.soil.wetnessDuration;
        }

    }


    //returns true if the plant is at max growth a.k.a. harvestable
    public bool Harvestable()
    {
        return isHarvestable;
    }

    //change texture to be withered or make it fresh again
    public void Wither()
    {
        //someday this'll be something like "plantObject texure = plant.witheredTexture"
        //for now just make it yellow
        currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.withered;
        withered = true;
    }
    public void Unwither()
    {
        currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.healthy;
        withered = false;
    }

    //add the correct items to the player's inventory and then unregisters and destroys the plant
    public void Harvest()
    {
        //tutorial
        if (TutorialManager.instance != null && !TutorialManager.instance.harvesting)
        {
            //completes billboard 3: plant seeds
            TutorialManager.instance.ProgressTutorial(5);
            TutorialManager.instance.harvesting = true;
        }

        InventoryManager.instance.invSO.AddItem(plant.fruit, 1);

        Debug.Log("Harvested");
        Destroy(currentPlant);
        TimeManager.instance.UnregisterPlant(this);
        Destroy(myCanvas);
        Destroy(this);
    }

    //same as harvest but doesn't add anything to the player's inventory
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
