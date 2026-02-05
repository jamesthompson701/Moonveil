using UnityEngine;


//this makes the UI face the main camera
//MAKE SURE THE CANVAS SCALE IS SET TO X -1 Y 1 Z -1
public class CanvasFacesPlayer : MonoBehaviour
{
    //player cam
    public Camera playerCam;

    private void Awake()
    {
        playerCam = PlayerCamera.instance.myCamera;
    }

    private void LateUpdate()
    {
        gameObject.transform.LookAt(playerCam.transform);
    }
}
