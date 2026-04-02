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
    public bool isDead;

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

        //tutorial
        if (!TutorialManager.instance.plantingDone)
        {
            TutorialManager.instance.ProgressTutorial(4);
            TutorialManager.instance.plantingDone = true;
        }
    }

    public void CheckPlant(float deltaTime)
    {
        if (!isSet)
        {
            //set the plant SO correctly based on the seed used
            growthTime = plant.cropTime;
            dryTime = plant.droughtResistance;
            currentPlant = Instantiate(plant.GetPrefabByStage(currentStage), transform);
            isSet = true;
        }

        //increment the dry timer while dry
        if(!soilScript.Wet())
        {

            //if it's been dry too long, it withers and stops growing
            if (dryTime > 0 && !isHarvestable)
            {
                dryTime = dryTime - deltaTime;

            }
            else
            {
                Wither();
            }
        }

        //update growth time
        if(growthTime > 0 && soilScript.Wet())
        {
            growthTime = growthTime - deltaTime;
            if(!isHarvestable)
            {
                //if plant is growing that means it's time to unwither it
                //but if it's harvestable just keep it green
                Unwither();
            }

        }
        else
        {
            //check wetness and make sure the plant is alive before growing
            if (soilScript.Wet())
            {
                Debug.Log("before growth: " + currentStage);
                growthTime = plant.cropTime;
                dryTime = plant.droughtResistance;

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

        //update growth timer UI and water timer UI
        growthTimer.text = "" + Mathf.Round(growthTime);
        growthProgressBar.fillAmount = growthTime / plant.cropTime;

        waterTimer.text = " " + Mathf.Round(soilScript.waterTimer);
        waterTimerBar.fillAmount = soilScript.waterTimer / soilScript.soil.wetnessDuration;

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
        isHarvestable = false;
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
        if (!TutorialManager.instance.harvestingDone)
        {
            TutorialManager.instance.ProgressTutorial(8);
            TutorialManager.instance.harvestingDone = true;
        }

        PlayerInventory.instance.invSO.AddItem(plant.fruit, 1);
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
