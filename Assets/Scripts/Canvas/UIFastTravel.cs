using UnityEngine;


public class UIFastTravel : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnFastTravelClicked()
    {
        EnvironmentManager.Instance.Travel(eFastTravel.home);
    }
}
