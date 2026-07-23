using Unity.VisualScripting;
using UnityEngine;

public class SoilObject : MonoBehaviour
{
    [SerializeField] public SoilSO soil;

    //what's in the soil (weed, crop, tilled, empty)
    public SoilContent soilContent;

    //current game object and plant object
    //the soil referse to the visual cube, not the empty object this script is attached to
    public GameObject mySoilObj;
    private GameObject plantObj;
    private PlantObject plantScript;

    //colors
    public Material wetSoil;
    public Material drySoil;
    public Material untilledSoil;

    //plant to generate and SO to use
    public GameObject plantPrefab;
    public PlantSO currentPlantSO;

    //fire particles
    public GameObject fire;

    //wetness timer
    public float waterTimer;
    public float wetnessDuration;

    //bools for tilled and wet
    public bool tilled;
    public bool isWet;

    private void Start()
    {
        //Register myself with the time manager
        TimeManager.instance.RegisterSoil(this);
        wetnessDuration = 120;
    }

    public void CheckSoil(float deltaTime)
    {
        if (currentPlantSO != null)
        {
            wetnessDuration = currentPlantSO.droughtResistance;
        }
        else { wetnessDuration = 120; }

        //if the soil is tilled or untilled, update it accordingly
        if (!tilled)
        {
            waterTimer = 0;
            isWet = false;
            mySoilObj.GetComponent<MeshRenderer>().material = untilledSoil;
        }
        else
        {
            mySoilObj.GetComponent<MeshRenderer>().material = drySoil;

            //if the soil is wet, make it the wet material and check how long ago it was watered
            if (isWet)
            {
                if (waterTimer > wetnessDuration)
                {
                    waterTimer = wetnessDuration;
                }

                if (plantScript != null)
                {
                    if (!plantScript.Withered())
                    {
                        waterTimer = waterTimer - deltaTime;
                    }
                }


                mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;

                //if its wetness time is up, make it dry
                if (waterTimer <= 0)
                {
                    isWet = false;
                }
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //if it's a water spell, soil becomes wet
        if (other.CompareTag("WateringSpell") && tilled)
        {
            isWet = true;
            waterTimer = wetnessDuration;
            mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;
        }

        if (other.CompareTag("WateringSpellSmall") && tilled)
        {
            isWet = true;
            waterTimer = wetnessDuration;
            mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("TillSpell"))
        {
            if(!tilled && soilContent == SoilContent.empty)
            {
                tilled = true;
            }
        }

        //if it's a harvest spell, harvest if able
        if (other.CompareTag("HarvestSpell") && plantScript != null)
        {
            if (plantScript.Harvestable())
            {
                plantScript.Harvest();
                soilContent = SoilContent.empty;
                tilled = false;
            }
        }

        //if it's a fire spell, destroy crop unless it's watered
        //unwater it if it is watered
        if (other.CompareTag("FireSpell") && plantScript != null)
        {
            if(!isWet && !plantScript.Harvestable())
            {
                Debug.Log("FireSpelled");
                Destroy(other.gameObject);
                plantScript.Destroy();
                soilContent = SoilContent.empty;
                Instantiate(fire, transform);
            }
            else
            {
                waterTimer = 0;
            }

        }

    }

    //return wetness (used by plant)
    public bool Wet()
    {
        return isWet;
    }

    //spawns a crop and sets it up to be connected with this soil script
    public void SpawnCrop()
    {
        Debug.Log("crop spawned");
        plantObj = Instantiate(currentPlantSO.prefab, gameObject.transform.position, gameObject.transform.rotation);
        plantScript = plantObj.GetComponent<PlantObject>();
        plantScript.plant = currentPlantSO;
        plantScript.SetSoil(this);
        soilContent = SoilContent.crop;
    }

    //set the plantSO to the SO taken from the seed item the player is currently weilding
    public void SetPlantType(SeedItemSO _plantData)
    {
        currentPlantSO = _plantData.plantType;
    }
}
