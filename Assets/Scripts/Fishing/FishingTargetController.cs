using UnityEngine;

public class FishingTargetController : MonoBehaviour
{
    public float speed = 8f;
    public FishingCameraController cameraController;

    void Update()
    {
        Debug.Log("FishingTargetController running");

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Debug.Log(cameraController);

        Transform cam = cameraController.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * z + right * x;

        Debug.Log($"x={x}, z={z}");

        transform.position += move * speed * Time.deltaTime;

        Debug.Log(move);
    }
}