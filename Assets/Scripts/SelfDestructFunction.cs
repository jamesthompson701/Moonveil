using UnityEngine;

public class SelfDestructFunction : MonoBehaviour
{
    public bool isPartOfTutorial;
    private bool isTutorialDone;
    public void SelfDestruct()
    {
        if (isPartOfTutorial)
        {
            if(!isTutorialDone)
            {
                TutorialManager.instance.ProgressTutorial();
                isTutorialDone = true;
            }
        }

        // I WANNA MAKE MY MURDER LOOK LIKE A SUICIDE
        Destroy(this.gameObject);
    }

}
