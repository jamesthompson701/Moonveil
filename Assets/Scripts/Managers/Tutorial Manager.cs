using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //billboards to be activated and deactivated
    public GameObject billboard0;
    public Transform point0;
    public GameObject billboard1;
    public Transform point1;
    public GameObject billboard2;
    public Transform point2;
    public GameObject billboard3;
    public Transform point3;
    public GameObject billboard4;
    public Transform point4;
    public GameObject billboard5;
    public Transform point5;
    public GameObject billboard6;
    public Transform point6;
    public GameObject billboard7;
    public Transform point7;
    public GameObject billboard8;
    public Transform point8;
    public GameObject billboard9;
    public Transform point9;
    public GameObject billboard10;
    public Transform point10;
    public GameObject billboard11;
    public Transform point11;
    public GameObject billboard12;
    public Transform point12;

    //tutorial arrow
    public GameObject arrow;
    private PointToTarget pointer;

    //variable to keep track of which stage of the tutorial they're on
    public int currentBillboard = 0;

    //bools
    public bool planting; //located in PlantObject
    public bool watering; //located in SoilObject
    public bool harvesting; //located in PlantObject
    public bool fishing; //located in FishingManager
    public bool fastTravel; //located in UIFastTravel
    public bool mining; //located in MiningManager
    public bool crafting; //located in CraftingManager
    public bool combat; //located in CreatureDefs

    //singleton
    public static TutorialManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        pointer = arrow.GetComponent<PointToTarget>();
    }

    //function that activates one billboard and deactivates another depending on the input
    public void ProgressTutorial(int _completedBillboard)
    {
        //increment the stage of the tutorial (the number inputted should be the billboard whose instructions were just completed)
        currentBillboard = _completedBillboard;

        switch (currentBillboard)
        {
            case 0:
                //movement done
                billboard0.SetActive(false);
                pointer.ChangeTarget(point1);
                billboard1.SetActive(true);
                break;
            case 1:
                //prep garden done (located in TimeManager)
                billboard1.SetActive(false);
                pointer.ChangeTarget(point2);
                billboard2.SetActive(true);
                break;
            case 2:
                //go forage done
                billboard2.SetActive(false);
                pointer.ChangeTarget(point3);
                billboard3.SetActive(true);
                break;
            case 3:
                //plant seeds done
                billboard3.SetActive(false);
                pointer.ChangeTarget(point4);
                billboard4.SetActive(true);
                break;
            case 4:
                //water plants done
                billboard4.SetActive(false);
                pointer.ChangeTarget(point5);
                billboard5.SetActive(true);
                break;
            case 5:
                //harvest plants done
                billboard5.SetActive(false);
                pointer.ChangeTarget(point6);
                billboard6.SetActive(true);
                break;
            case 6:
                //fih done
                billboard6.SetActive(false);
                pointer.ChangeTarget(point7);
                billboard7.SetActive(true);
                break;
            case 7:
                //teleport to the mines done
                billboard7.SetActive(false);
                pointer.ChangeTarget(point8);
                billboard8.SetActive(true);
                break;
            case 8:
                //mine a gem
                billboard8.SetActive(false);
                pointer.ChangeTarget(point9);
                billboard9.SetActive(true);
                break;
            case 9:
                //craft anything
                billboard9.SetActive(false);
                pointer.ChangeTarget(point10);
                billboard10.SetActive(true);
                break;
            case 10:
                //go to fire island
                billboard10.SetActive(false);
                pointer.ChangeTarget(point11);
                billboard11.SetActive(true);
                break;
            case 11:
                //kill a dog
                billboard11.SetActive(false);
                pointer.ChangeTarget(point12);
                billboard12.SetActive(true);
                break;
            case 12:
                //give the NPC his shark idol
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
                Destroy(arrow);

                SpellManager2.Instance.waterTierUnlocked[1] = true;

                Debug.Log("Tutorial COMPLETED");

                //its purpose fulfilled, Tutorial Manager immediately commits suicide to prevent the dishonor of burdening its family in old age
                Destroy(this.gameObject);
                break;
                
        }

    }
}
