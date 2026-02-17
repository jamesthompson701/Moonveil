using UnityEngine;


public class UIFastTravel : MonoBehaviour
{

    public void OnFastTravelClicked()
    {
        EnvironmentManager.Instance.Travel(destination);
    }
}
