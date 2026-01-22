using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{

    //list of soil spots
    private List<SoilObject> soilObjects = new List<SoilObject>();

    public static SoilManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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
}
