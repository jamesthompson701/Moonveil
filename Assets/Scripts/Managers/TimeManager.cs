using UnityEngine;
using System.Collections.Generic;


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
    private float time;

    public static TimeManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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

    public void Update()
    {
        time = Time.deltaTime;

        //check each plant in the list
        foreach (PlantObject plantObject in plantObjects)
        {
            plantObject.CheckPlant(time);
        }

        //check each soil spot in the list
        foreach (SoilObject soilObject in soilObjects)
        {
            soilObject.CheckSoil(time);
        }
    }
}
