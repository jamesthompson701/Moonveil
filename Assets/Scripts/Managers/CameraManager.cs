using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private bool rotatePlayerYaw = true; // if false, only camera rotates (player doesn’t)
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float pitch;
    private float yaw;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = GetComponentInChildren<Camera>().transform ?? transform;

        yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (rotatePlayerYaw && player != null)
            player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
