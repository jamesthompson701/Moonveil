using UnityEngine;

public class FishingCameraController : MonoBehaviour
{
    public Transform target;

    public float rotateSpeed = 100f;

    float yaw;
    float pitch;

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        pitch -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -45f, 60f);

        transform.rotation = Quaternion.Euler(pitch,yaw,0f);
    }
}
