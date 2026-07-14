using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
    public GameObject popup;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;
    }
    public void OpenTutorial()
    {
    popup.SetActive(true);
        
    }

    public void CloseTutorial()
    {
    popup.SetActive(false);
    }
}
