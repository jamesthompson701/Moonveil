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

    //weed to generate
    public GameObject weedObj;

    //colors
    public Material wetSoil;
    public Material drySoil;

    //plant to generate (temporary)
    public GameObject plantPrefab;

    //current time
    private float currentTime;

    //wet bool and wetness timer
    public bool isWet = false;
    public float wateredAtTime;
    public float wetnessDuration;

    private void Start()
    {
        //if this has been predetermined as a crop, spawn a plant
        //for testing purposes only; normal plants will probably be spawned through other means
        if (soilContent == SoilContent.crop)
        {
            Debug.Log("crop spawned");
            plantObj = Instantiate(plantPrefab, gameObject.transform.position, gameObject.transform.rotation);
            plantScript = plantObj.GetComponent<PlantObject>();
            plantScript.SetSoil(this);
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
                Instantiate(weedObj, gameObject.transform.position, gameObject.transform.rotation);
            }
        }
    }

    public void CheckSoil(float deltaTime)
    {
        //update currentTime
        currentTime = currentTime + deltaTime;

        //if the soil is wet, make it the wet material and check how long ago it was watered
        if (isWet == true)
        {
            mySoilObj.GetComponent<MeshRenderer>().material = wetSoil;
            //if it's been wet for longer than the wetness duration, make it dry
            if (currentTime - wateredAtTime > wetnessDuration)
            {
                currentTime = 0;
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
            wateredAtTime = currentTime;
            Debug.Log("Soil wet");
        }
        if (other.CompareTag("HarvestSpell") && plantScript != null)
        {
            if (plantScript.Harvestable())
            {
                plantScript.Harvest();
            }
        }

    }
}
