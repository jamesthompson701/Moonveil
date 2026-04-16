using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //billboards to be activated and deactivated
    public GameObject billboard0;
    public GameObject billboard1;
    public GameObject billboard2;
    public GameObject billboard3;
    public GameObject billboard4;
    public GameObject billboard5;
    public GameObject billboard6;
    public GameObject billboard7;
    public GameObject billboard8;
    public GameObject billboard9;
    public GameObject billboard10;
    public GameObject billboard11;
    public GameObject billboard12;
    //public GameObject billboard13;
    //public GameObject billboard14;

    //tutorial arrow
    public GameObject arrow;

    //variable to keep track of which stage of the tutorial they're on
    public int currentBillboard = 0;

    //other variables
    public bool inventoryDone;
    public bool plantingDone;
    public bool fishingDone;
    public bool harvestingDone;
    public bool fastTravelDone;
    public bool combatDone;
    public bool craftingDone;

    //singleton
    public static TutorialManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //function that activates one billboard and deactivates another depending on the input
    public void ProgressTutorial(int _nextBillboard)
    {
        //increment the stage of the tutorial
        currentBillboard = _nextBillboard;

        switch (currentBillboard)
        {
            case 0:
                billboard0.SetActive(false);
                movementTrigger.SetActive(true);
                billboard1.SetActive(true);
                break;
            case 1:
                billboard1.SetActive(false);
                billboard2.SetActive(true);
                break;
            case 2:
                billboard2.SetActive(false);
                billboard3.SetActive(true);
                break;
            case 3:
                billboard3.SetActive(false);
                billboard4.SetActive(true);
                break;
            case 4:
                billboard4.SetActive(false);
                fishingTrigger.SetActive(true);
                billboard5.SetActive(true);
                break;
            case 5:
                billboard5.SetActive(false);
                billboard6.SetActive(true);
                break;
            case 6:
                billboard6.SetActive(false);
                harvestTrigger.SetActive(true);
                billboard7.SetActive(true);
                break;
            case 7:
                billboard7.SetActive(false);
                billboard8.SetActive(true);
                break;
            case 8:
                billboard8.SetActive(false);
                combatTrigger.SetActive(true);
                billboard9.SetActive(true);
                break;
            case 9:
                billboard9.SetActive(false);
                billboard10.SetActive(true);
                break;
            case 10:
                billboard10.SetActive(false);
                backToFarmingTrigger.SetActive(true);
                billboard11.SetActive(true);
                break;
            case 11:
                billboard11.SetActive(false);
                billboard12.SetActive(true);
                break;
            case 12:
                billboard1.SetActive(false);
                billboard2.SetActive(false);
                billboard3.SetActive(false);
                billboard4.SetActive(false);
                billboard5.SetActive(false);
                billboard6.SetActive(false);
                billboard7.SetActive(false);
                billboard8.SetActive(false);
                billboard10.SetActive(false);
                billboard11.SetActive(false);
                billboard12.SetActive(false);

                SpellManager2.Instance.waterTierUnlocked[1] = true;

                Debug.Log("Tutorial COMPLETED");

                //its purpose fulfilled, Tutorial Manager immediately commits suicide to prevent the dishonor of burdening its family in old age
                Destroy(this.gameObject);
                break;
        }

    }
}
