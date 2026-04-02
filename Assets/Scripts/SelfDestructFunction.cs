using UnityEngine;

public class SelfDestructFunction : MonoBehaviour
{
    public bool isPartOfTutorial;
    public void SelfDestruct()
    {
        if (isPartOfTutorial)
        {
            if (!TutorialManager.instance.movementTriggerDone)
            {
                TutorialManager.instance.ProgressTutorial(1);
                TutorialManager.instance.movementTriggerDone = true;
            }
            else if(!TutorialManager.instance.fishingTriggerDone)
            {
                TutorialManager.instance.ProgressTutorial(5);
                TutorialManager.instance.fishingTriggerDone = true;
            }
            else if (!TutorialManager.instance.harvestTriggerDone)
            {
                TutorialManager.instance.ProgressTutorial(7);
                TutorialManager.instance.harvestTriggerDone = true;
            }
            else if (!TutorialManager.instance.combatTriggerDone)
            {
                TutorialManager.instance.ProgressTutorial(9);
                TutorialManager.instance.combatTriggerDone = true;
            }
            else if (!TutorialManager.instance.backToFarmingDone)
            {
                TutorialManager.instance.ProgressTutorial(11);
                TutorialManager.instance.backToFarmingDone = true;
            }
        }

        // I WANNA MAKE MY MURDER LOOK LIKE A SUICIDE
        Destroy(this.gameObject);
    }

}
