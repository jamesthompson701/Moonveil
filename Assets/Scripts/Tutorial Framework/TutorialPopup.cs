using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public GameObject popup;

    public void OpenTutorial()
{
    popup.SetActive(true);
}

public void CloseTutorial()
{
    popup.SetActive(false);
}
}
