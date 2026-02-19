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

    //harvestability
    private bool isHarvestable;

    //plant object & soil
    private GameObject currentPlant;
    private SoilObject soilScript;

    //canvas and growth timer
    public Canvas myCanvas;
    public TMP_Text growthTimer;
    public Image growthProgressBar;

    //bool to toggle if it's been setup
    private bool isSet;

    void Awake()
    {
        //add to time manager
        currentStage = 0;
        TimeManager.instance.RegisterPlant(this);

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

        //if the soil is dry, this functionally pauses the growth timer by negating it
        //increment the dry timer while dry
        if(!soilScript.Wet())
        {
            growthTime = growthTime + deltaTime;

            //if it's been dry too long, then it dies
            if (dryTime > 0)
            {
                dryTime = dryTime - deltaTime;

            }
            else
            {
                isDead = true;
                isHarvestable = false;
                Destroy(myCanvas);
                Destroy(currentPlant);
                currentPlant = Instantiate(plant.plantDead, transform);
            }
        }

        //update growth time
        if(growthTime > 0)
        {
            growthTime = growthTime - deltaTime;
        }
        else
        {
            //check wetness and make sure the plant is alive before growing
            if (soilScript.Wet() && !isDead)
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

        //update growth timer UI
        growthTimer.text = "" + Mathf.Round(growthTime);
        growthProgressBar.fillAmount = growthTime / plant.cropTime;

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
