using UnityEngine;

public class SoilObject : MonoBehaviour
{
    //don't need this yet
    //[SerializeField] private SoilSO soil;

    //public for now so it can be tested, but eventually this'll default to empty
    public SoilContent soilContent;

    //current game object
    private GameObject currentSoil;

    //weed to generate
    public GameObject weedObj;

    //plant to generate (temporary)
    public GameObject plantObj;

    //if the soil is a crop, make a crop there
    //otherwise, roll a number and generate a weed on a 7
    //this is a temporary system for the demo but honestly? randomly rerolling every non-crop tile
    //might not be a bad idea for the final game
    private void Start()
    {
        if (soilContent == SoilContent.crop)
        {
            Debug.Log("crop spawned");
            Instantiate(plantObj, gameObject.transform.position, gameObject.transform.rotation);
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
                Debug.Log("weed spawned");
                Instantiate(weedObj, gameObject.transform.position, gameObject.transform.rotation);
            }
        }

    }
}
