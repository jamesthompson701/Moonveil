using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera myCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
