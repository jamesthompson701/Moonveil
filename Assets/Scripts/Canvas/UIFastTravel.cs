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
        Debug.Log("Fast Travel Clicked");
        EnvironmentManager.Instance.Travel((eFastTravel)destination);
        CanvasManager.Instance.OpenFastTravel();
    }
}
