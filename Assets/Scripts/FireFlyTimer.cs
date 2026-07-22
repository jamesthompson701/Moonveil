using UnityEngine;

public class FireFlyTimer : MonoBehaviour
{
    void Update()
    {
        if (TimeManager.instance == null) return;
        if (TimeManager.instance.dayLength >= 300)
        {
            gameObject.SetActive(true);
        }
        if (TimeManager.instance.dayLength < 300 && TimeManager.instance.dayLength >=0)
        {
            gameObject.SetActive(false);
        }
    }
}
