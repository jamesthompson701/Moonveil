using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //billboards to be activated and deactivated
    public GameObject billboard1;
    public GameObject billboard2;
    public GameObject billboard3;
    public GameObject billboard4;
    public GameObject billboard5;
    public GameObject fishingTrigger;
    public GameObject billboard6;
    public GameObject billboard7;
    public GameObject billboard8;
    public GameObject billboard9;

    //variable to keep track of which stage of the tutorial they're on
    public int currentBillboard = 1;

    //other variables
    public bool inventoryDone;
    public bool plantingDone;

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
    public void ProgressTutorial()
    {
        switch (currentBillboard)
        {
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
        }

        //increment the stage of the tutorial
        currentBillboard++;
    }
}
