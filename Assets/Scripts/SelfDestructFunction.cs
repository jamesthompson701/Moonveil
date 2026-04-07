using System.Collections;
using UnityEngine;

public class SelfDestructFunction : MonoBehaviour
{
    public bool isPartOfTutorial;
    public void SelfDestruct()
    {
        if (isPartOfTutorial)
        {
            switch((TutorialManager.instance.currentBillboard))
            {
                case 0:
                    Debug.Log("Just ran progress 1");
                    TutorialManager.instance.ProgressTutorial(1);
                    break;
                case 4:
                    TutorialManager.instance.ProgressTutorial(5);
                    break;
                case 6:
                    TutorialManager.instance.ProgressTutorial(7);
                    break;
                case 8:
                    TutorialManager.instance.ProgressTutorial(9);
                    break;
                case 10:
                    TutorialManager.instance.ProgressTutorial(11);
                    break;

            }

        }

        // I WANNA MAKE MY MURDER LOOK LIKE A SUICIDE
        Destroy(this.gameObject);
    }

}
