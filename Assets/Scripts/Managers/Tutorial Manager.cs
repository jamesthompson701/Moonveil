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
    public GameObject billboard6;
    public GameObject billboard7;
    public GameObject billboard8;
    public GameObject billboard9;

    //variable to keep track of which stage of the tutorial they're on
    public int currentBillboard = 1;

    //function that activates one billboard and deactivates another depending on the input
    public void ProgressTutorial()
    {
        switch (currentBillboard)
        {
            case 1:
                billboard1.SetActive(false);
                billboard2.SetActive(true);
                break;

        }

        //increment the stage of the tutorial
        currentBillboard++;
    }
}
