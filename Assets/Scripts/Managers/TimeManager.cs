using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;
using System.Collections;
using UnityEditorInternal;


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
    private float dayLength = 600f;

    // seperate time for day/night cycle
    public float daylightCycleTime = 1;

    // world light
    public GameObject worldLight;
    public Light sun;

    public static TimeManager instance;

    // skybox
    public Material night;
    public Material day;

    private float skyboxTransitionStatus = 0;

    //tutorial
    public bool tutorialDone;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timeOfDay = 1;
        RenderSettings.skybox = day;
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
        // ONLY WORKS WHILE DEBUG MENU IS ENABLED

        if (DebugCanvas.instance.gameObject.activeInHierarchy)
        {
            if (timeOfDay == 1)
            {
                daylightCycleTime = 300;
            }
            if (timeOfDay == 2)
            {
                daylightCycleTime = 600;
            }
        }
    }

    public void Update()
    {
        time = Time.deltaTime;
        daylightCycleTime = daylightCycleTime + time;

        //rotate the sky
        switch(timeOfDay)
        {
            case 1:
                if (sun.intensity < 3 )
                {
                    sun.intensity = sun.intensity + 0.01f;
                }
                worldLight.transform.Rotate(0.6f * Time.deltaTime,0,0);
                //RenderSettings.skybox = day;
                Debug.Log(skyboxTransitionStatus);
                RenderSettings.skybox.Lerp(day, night, skyboxTransitionStatus);
                if (skyboxTransitionStatus > 0)
                {
                    skyboxTransitionStatus = skyboxTransitionStatus - 0.1f;
                }
                break;
            case 2:
                if (sun.intensity > 0)
                {
                    sun.intensity = sun.intensity - 0.01f;
                }
                //RenderSettings.skybox = night;
                Debug.Log(skyboxTransitionStatus);
                RenderSettings.skybox.Lerp(day, night, skyboxTransitionStatus);
                if (skyboxTransitionStatus > 0)
                {
                    skyboxTransitionStatus = skyboxTransitionStatus - 0.1f;
                }
                break;
        }

        //update the time of day
        if (daylightCycleTime > 300)
        {
            timeOfDay = 2;
        }
        if (daylightCycleTime > 600)
        {
            daylightCycleTime = 1;
            timeOfDay = 1;
            worldLight.transform.rotation = new Quaternion(0, 0, 0, 0);
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


        //if tutorial hasn't been completed, then check if all the soil has been tilled
        //if they're all tilled, progress the tutorial
        if (!tutorialDone)
        {
            int untilled = 0;

            foreach (SoilObject soilObject in soilObjects)
            {
                if (!soilObject.tilled)
                {
                    untilled++;
                }
            }
            if (untilled == 0)
            {
                if (TutorialManager.instance.currentBillboard == 0)
                {
                    TutorialManager.instance.ProgressTutorial(1);
                    tutorialDone = true;
                }

            }

        }

    }
}
