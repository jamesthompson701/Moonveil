using UnityEngine;

public class SoilObject : MonoBehaviour
{
    //don't need this yet
    //[SerializeField] private SoilSO soil;

    //public for now so it can be tested, but eventually this'll default to empty
    public SoilContent soilContent;

    //current game object and plant object
    private GameObject currentSoil;
    private GameObject myPlant;
    private PlantObject myPlantObj;

    //weed to generate
    public GameObject weedObj;

    //plant to generate (temporary)
    public GameObject plantObj;

    //wet bool
    public bool isWet = false;

    //if the soil is a crop, make a crop there
    //otherwise, roll a number and generate a weed on a 7
    private void Start()
    {
        if (soilContent == SoilContent.crop)
        {
            Debug.Log("crop spawned");
            myPlant = Instantiate(plantObj, gameObject.transform.position, gameObject.transform.rotation);
        }
        else
        {
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


    private void OnTriggerEnter(Collider other)
    {
        //if it's a water spell, soil becomes wet
        if (other.CompareTag("WateringSpell"))
        {
            isWet = true;
        }

    }
}
