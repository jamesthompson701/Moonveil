using UnityEngine;


//this makes the UI face the main camera
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
        //actually faces the opposite direction of the camera because worldcanvas is weird and mirrored
        var direction = gameObject.transform.position - playerCam.transform.position;
        var lookRotation = Quaternion.LookRotation(direction);
        gameObject.transform.LookAt(playerCam.transform);
        gameObject.transform.rotation = lookRotation;
    }
}
