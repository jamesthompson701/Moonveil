using UnityEngine;

public class SoilObject : MonoBehaviour
{
    //don't need this yet
    //[SerializeField] private SoilSO soil;

    //public for now so it can be tested, but eventually this'll default to empty
    public SoilContent soilContent;

    //current game object and plant object
    //the soil referse to the visual cube, not the empty object this script is attached to
    public GameObject mySoilObj;
    private GameObject plantObj;
    private PlantObject plantScript;

    //weed to generate and weed that has been generated
    public GameObject weedObj;
    private GameObject weed;

    //colors
    public Material wetSoil;
    public Material drySoil;

    //plant to generate (temporary)
    public GameObject plantPrefab;

    //wet bool and wetness timer
    public bool isWet = false;
    private float waterTimer;
    public float wetnessDuration;

    private void Start()
    {
        //Register myself with the time manager
        TimeManager.instance.RegisterSoil(this);

        //if this has been predetermined as a crop, spawn a plant
        //for testing purposes only; normal plants will probably be spawned through other means
        if (soilContent == SoilContent.crop)
        {
            SpawnCrop();
        }
        else
        {
            //randomly generate a weed if it isn't a crop square
            int randomNum = Random.Range(3, 8);
            if (randomNum == 7)
            {
                soilContent = SoilContent.weed;
            }
            if (soilContent == SoilContent.weed)
            {
                weed = Instantiate(weedObj, gameObject.transform.position, gameObject.transform.rotation);
            }
        }
    }

    public void CheckSoil(float deltaTime)
    {

        //if the soil is wet, make it the wet material and check how long ago it was watered
        if (isWet)
        {
            waterTimer = waterTimer + deltaTime;
            mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;
            Debug.Log("its wet its working");

            //if it's been wet for longer than the wetness duration, make it dry
            if (waterTimer > wetnessDuration)
            {
                waterTimer = 0;
                isWet = false;
                mySoilObj.GetComponent<MeshRenderer>().material = drySoil;
                Debug.Log("Soil dry");
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //if it's a water spell, soil becomes wet
        if (other.CompareTag("WateringSpell"))
        {
            isWet = true;
            mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;
            Debug.Log("Soil wet");
        }

        if (other.CompareTag("TillSpell"))
        {
            //nothing here yets
        }

        //if it's a harvest spell, harvest if able
        if (other.CompareTag("HarvestSpell") && plantScript != null)
        {
            if (plantScript.Harvestable())
            {
                plantScript.Harvest();
                soilContent = SoilContent.empty;
            }
        }

        //if it's a fire spell, destroy crop unless it's watered
        if (other.CompareTag("FireSpell") && !isWet && plantScript != null)
        {
            Debug.Log("FireSpelled");
            plantScript.Destroy();
            soilContent = SoilContent.empty;
        }

        //fire spell destroys weeds also
        if (other.CompareTag("FireSpell") && soilContent == SoilContent.weed)
        {
            Destroy(weed);
            soilContent = SoilContent.empty;
        }

    }

    //return wetness (used by plant)
    public bool Wet()
    {
        return isWet;
    }

    //spawns a crop
    //later, may need to take input to determine what kind of crop
    public void SpawnCrop()
    {
        Debug.Log("crop spawned");
        plantObj = Instantiate(plantPrefab, gameObject.transform.position, gameObject.transform.rotation);
        plantScript = plantObj.GetComponent<PlantObject>();
        plantScript.SetSoil(this);
        soilContent = SoilContent.crop;
    }
}
