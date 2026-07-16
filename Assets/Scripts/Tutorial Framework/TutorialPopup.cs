using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
    public GameObject popup;
    public GameObject nextQuest;
    public TutorialStep previousQuest;

    private void OnEnable()
    {
        SpellManager2.Instance.inMenu = true;
        CanvasManager.Instance.playerMap.Disable();
        CanvasManager.Instance.UIMap.Enable();

        Time.timeScale = 0f;

        CanvasManager.Instance.starterAssets.cursorLocked = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
    }
    private void OnDisable()
    {
        
        
    }
    public void OpenTutorial()
    {
        popup.SetActive(true);
    }

    public void CloseTutorial()
    {
        SpellManager2.Instance.inMenu = false;
        CanvasManager.Instance.playerMap.Enable();
        CanvasManager.Instance.UIMap.Disable();

        CanvasManager.Instance.starterAssets.cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;

        Time.timeScale = 1f;

        if (previousQuest != null)
            previousQuest.CompleteStep();

        if (nextQuest != null)
            nextQuest.SetActive(true);

        popup.SetActive(false);
    }
}
