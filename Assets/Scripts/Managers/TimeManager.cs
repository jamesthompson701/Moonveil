using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;
using System.Collections;
//using UnityEditorInternal;
using TMPro;
using UnityEngine.UI;


//This is the Universal Time Manager
//It keeps a list of all the crops and soil objects in existence and updates their status in Update
//Also contains functions to add and remove plants from the list

public class TimeManager : MonoBehaviour
{
    //list of plants
    public List<PlantObject> plantObjects = new List<PlantObject>();

    //list of soil spots
    private List<SoilObject> soilObjects = new List<SoilObject>();

    //Time
    public float time;

    // Time of day
    //1 = day, 2 = night
    public int timeOfDay;

    //rotation time of day (only used by TimeManager)
    // 1-evening 2-night 3-sunrise 4-morning
    private int rotationTimeOfDay;

    //length of day in seconds
    public float dayLength = 600f;

    // seperate time for day/night cycle
    public float daylightCycleTime = 1;

    // world light
    public GameObject worldLight;
    public Light sun;
    public GameObject clockHand;

    //blackout screen
    public Image blackout;
    public bool isBlackout;
    public float blackoutTimer;

    public static TimeManager instance;

    // skybox
    public Material night;
    public Material day;

    private float currentBlend;

    //tutorial
    public bool tilledDone;
    public bool plantDone;
    public bool waterDone;
    public bool harvestDone;
    public bool plantingTutorialComplete;
    public bool hitBushDone;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timeOfDay = 1;
    }

    //add or remove plants
    public void RegisterPlant(PlantObject plantObject)
    {
        plantObjects.Add(plantObject);
    }
    public void UnregisterPlant(PlantObject plantObject)
    {
        plantObjects.Remove(plantObject);
    }

    //add or remove soil spots
    public void RegisterSoil(SoilObject soilObject)
    {
        soilObjects.Add(soilObject);
    }
    public void UnregisterSoil(SoilObject soilObject)
    {
        soilObjects.Remove(soilObject);
    }

    public void Sleep()
    {
        // Sleeping immediately swaps from day to night, and vice versa
        //Witches don't sleep, they drink
            if (timeOfDay == 1)
            {
                daylightCycleTime = 300;
            }
            if (timeOfDay == 2)
            {
                daylightCycleTime = 600;
            }

    }

    public void Update()
    {
        //TUTORIAL STUFF START
        foreach (SoilObject soil in soilObjects)
        {
            if(soil.tilled)
            {
                tilledDone = true;
                TutorialEvents.TriggerTill();
            }
            if(soil.isWet)
            {
                waterDone = true;
                TutorialEvents.TriggerWater();
            }
        }
        if (plantObjects.Count > 0)
        {
            plantDone = true;
            TutorialEvents.TriggerPlant();
        }
        if (!harvestDone || !hitBushDone)
        {
            foreach (InventoryItem item in InventoryManager.instance.invSO.InventoryItems)
            {
                switch (item.item.itemID)
                {
                    case 0:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 1:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 2:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 3:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 4:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 5:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 6:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 7:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 12:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 13:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 14:
                        if (!harvestDone)
                        {
                            TutorialEvents.TriggerHarvest();
                            harvestDone = true;
                        }
                        break;
                    case 23:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 24:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 25:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 26:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;
                    case 27:
                        if (!hitBushDone)
                        {
                            TutorialEvents.TriggerHitBush();
                            hitBushDone = true;
                        }
                        break;

                }
            }
            if (tilledDone && plantDone && waterDone && harvestDone)
            {
                plantingTutorialComplete = true;
            }
        }
        //TUTORIAL STUFF END

        time = Time.deltaTime;
        daylightCycleTime = daylightCycleTime + time;

        if (isBlackout)
        {
            blackoutTimer -= Time.deltaTime;
            Color colorRef = blackout.color;
            colorRef.a = 255;
            blackout.color = colorRef;

            if (blackoutTimer <= 0f)
            {
                isBlackout = false;
                blackoutTimer = 0f;
            }
        }
        else
        {
            Color colorRef = blackout.color;
            colorRef.a = 0;
            blackout.color = colorRef;
        }

        //rotate the sky
        switch (timeOfDay)
        {
            case 1:
                if (sun.intensity < 3 )
                {
                    sun.intensity = sun.intensity + 0.01f;
                }
                worldLight.transform.Rotate(0.6f * Time.deltaTime, 0, 0);
                clockHand.transform.Rotate(0, 0, -1.2f * Time.deltaTime);

                if(currentBlend > 0)
                {
                    currentBlend = currentBlend - 0.01f;
                }
                RenderSettings.skybox.SetFloat("_Blend", currentBlend);
                break;
            case 2:
                if (sun.intensity > 0)
                {
                    sun.intensity = sun.intensity - 0.01f;
                }
                worldLight.transform.Rotate(0.6f * Time.deltaTime, 0, 0);
                clockHand.transform.Rotate(0, 0, -1.2f * Time.deltaTime);

                if (currentBlend < 1)
                {
                    currentBlend = currentBlend + 0.01f;
                }
                RenderSettings.skybox.SetFloat("_Blend", currentBlend);
                break;
        }

        //update the time of day
        if (daylightCycleTime > 300 && daylightCycleTime < 303)
        {
            timeOfDay = 2;
            worldLight.transform.eulerAngles = new Vector3(180, 180, 0);
            clockHand.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (daylightCycleTime > 600)
        {
            daylightCycleTime = 1;
            timeOfDay = 1;
            worldLight.transform.eulerAngles = new Vector3(0,180,0);
            clockHand.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        //check each plant in the list
        foreach (PlantObject plantObject in plantObjects)
        {
            plantObject.CheckPlant(time, timeOfDay);
        }

        //check each soil spot in the list
        foreach (SoilObject soilObject in soilObjects)
        {
            soilObject.CheckSoil(time);
        }

    }
}
