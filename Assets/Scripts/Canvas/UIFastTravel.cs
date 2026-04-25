using UnityEngine;


public class UIFastTravel : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnFastTravelClicked(int destination)
    {
        //tutorial
        if (TutorialManager.instance != null && !TutorialManager.instance.fastTravel)
        {
            //completes billboard 7; teleporting to the mines
            TutorialManager.instance.ProgressTutorial(7);
            TutorialManager.instance.fastTravel = true;
        }

        Debug.Log("Fast Travel Clicked");
        EnvironmentManager.Instance.Travel((eFastTravel)destination);
        CanvasManager.Instance.OpenMenu(5);
    }
}
