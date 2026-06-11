using UnityEngine;

public class FishingCameraController : MonoBehaviour
{
    public float rotateSpeed = 100f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up, mouseX * rotateSpeed * Time.deltaTime);

        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mouseY * rotateSpeed * Time.deltaTime);
    }
}
