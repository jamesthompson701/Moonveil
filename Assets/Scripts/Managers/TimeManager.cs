using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;


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
    //1 = morning, 2 = night
    public int timeOfDay;

    //rotation time of day (only used by TimeManager)
    // 1-evening 2-night 3-sunrise 4-morning
    private int rotationTimeOfDay;

    //length of day in seconds
    private float dayLength = 1200f;

    // seperate time for day/night cycle
    public float daylightCycleTime = 1;

    // world light
    public GameObject worldLight;

    public static TimeManager instance;

    //tutorial
    public bool tutorialDone;

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
        if (rotationTimeOfDay == 1)
        {
            daylightCycleTime = 750;
        }
        else if (rotationTimeOfDay == 2)
        {
            daylightCycleTime = 1150;
        }
        else if (rotationTimeOfDay == 3)
        {
            daylightCycleTime = 1200;
        }
        else if (rotationTimeOfDay == 4)
        {
            daylightCycleTime = 700;
        }
    }

    public void Update()
    {
        time = Time.deltaTime;
        daylightCycleTime = daylightCycleTime + time;

        //change time of day based on time
        if (daylightCycleTime >= 700 && daylightCycleTime < 750)
        {
            timeOfDay = 1;
            if (rotationTimeOfDay != 1)
            {
                worldLight.transform.Rotate(90, 0, 0);
                rotationTimeOfDay = 1;
            }

            HUD.instance.clockWheel.transform.eulerAngles = new Vector3(0, 0, 180f);
        }
        else if (daylightCycleTime >= 750 && daylightCycleTime < 1150)
        {
            timeOfDay = 2;
            if (rotationTimeOfDay != 2)
            {
                worldLight.transform.Rotate(90, 0, 0);
                rotationTimeOfDay = 2;
            }

            HUD.instance.clockWheel.transform.eulerAngles = new Vector3(0, 0, 67f);
        }
        else if (daylightCycleTime >= 1150 && daylightCycleTime < 1200)
        {
            timeOfDay = 1;
            if (rotationTimeOfDay != 3)
            {
                worldLight.transform.Rotate(90, 0, 0);
                rotationTimeOfDay = 3;
            } 

            HUD.instance.clockWheel.transform.eulerAngles = new Vector3(0, 0, 180f);
        }
        else if (daylightCycleTime >= dayLength)
        {
            daylightCycleTime = 0;
            timeOfDay = 1;
<<<<<<< Updated upstream
=======
            if (rotationTimeOfDay != 4)
            {
                worldLight.transform.Rotate(90, 0, 0);
                rotationTimeOfDay = 4;
            }
>>>>>>> Stashed changes

            HUD.instance.clockWheel.transform.eulerAngles = new Vector3(0, 0, 300f);
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
