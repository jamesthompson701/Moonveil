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

    //harvestability and withered status
    private bool isHarvestable;

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

    //is plant withered
    private bool withered;

    void Start()
    {
        //add to time manager
        currentStage = 0;
        TimeManager.instance.RegisterPlant(this);
    }

    //checkplant
    // light refers to the time of day; 1 = morning, 2 = night
    public void CheckPlant(float deltaTime, int _light)
    {
        if (!isSet)
        {
            //set the plant SO correctly based on the seed used
            growthTime = plant.cropTime;
            currentStage = 0;
            currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            isSet = true;
        }

        //wither plant while dry or if it's the wrong time
        if(!soilScript.Wet() || plant.lightPreference != _light)
        {
            if (!isHarvestable)
            {
                Wither();
            }
        }

        //update growth time as long as the soil is wet, the light is appropriate, and it isn't harvestable
        if (growthTime > 0 && soilScript.isWet && plant.lightPreference == _light && !isHarvestable)
        {
            if (!TimeManager.instance.plantingTutorialComplete)
            {
                growthTime = growthTime - deltaTime * 100;
            }
            else
            {
                growthTime = growthTime - deltaTime;
            }
            Unwither();
        }
        else if (growthTime <= 0)
        {
            //check wetness again before growing
            if (soilScript.isWet)
            {
                Debug.Log("before growth: " + currentStage);

                //reset growth timer
                growthTime = plant.cropTime;

                //then increment, but not past the max
                if (currentStage < 2)
                {
                    currentStage++;
                }
                else if (currentStage == 2)
                {
                    isHarvestable = true;
                    Unwither();
                    Destroy(myCanvas);
                    Debug.Log("Harvestable!");
                }

                //destroy the current object and make a new one at the new growth stage
                if (plant.GetPrefabByStage(currentStage) != null)
                {
                    Destroy(currentPlant);
                    currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
                    Unwither();
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
            waterTimerBar.fillAmount = soilScript.waterTimer / plant.droughtResistance;
        }
    }

    //returns true if the plant is at max growth a.k.a. harvestable
    public bool Harvestable()
    {
        return isHarvestable;
    }

    //returns true if the plant is withered a.k.a. dry or the time is wrong
    public bool Withered()
    {
        return withered;
    }

    //change texture to be withered or make it fresh again
    public void Wither()
    {
        withered = true;

        //change texture appropriately
        if (currentStage == 0)
        {
            currentPlant.GetComponent<MeshRenderer>().material = plant.dead1;
            currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.dead1;
        }
        if(currentStage == 1)
        {
            currentPlant.GetComponent<MeshRenderer>().material = plant.dead2;
            currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.dead2;
        }

    }
    public void Unwither()
    {
        withered = false;

        //change texture appropriately
        if (currentStage == 0)
        {
            currentPlant.GetComponent<MeshRenderer>().material = plant.healthy1;
            currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.healthy1;
        }
        else if (currentStage == 1)
        {
            currentPlant.GetComponent<MeshRenderer>().material = plant.healthy2;
            currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.healthy2;
        }
        else if (currentStage == 2)
        {
            currentPlant.GetComponent<MeshRenderer>().material = plant.healthy3;
            currentPlant.GetComponentInChildren<MeshRenderer>().material = plant.healthy3;
        }

    }

    //add the correct items to the player's inventory and then unregisters and destroys the plant
    public void Harvest()
    {
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
