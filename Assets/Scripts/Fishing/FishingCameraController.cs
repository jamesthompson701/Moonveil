using UnityEngine;

public class FishingCameraController : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float snapSpeed = 15f;

    float yaw;
    float pitch;

    private Quaternion targetRotation;
    private bool isSnapping;

    void Start()
    {
        SetRotationValues();
    }

    void Update()
    {
        if (isSnapping)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * snapSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                isSnapping = false;
                //SetRotationValues();
            }

            return;
        }


        yaw += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        pitch -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -45f, 60f);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    public void SmoothLookAt(Transform target)
    {
        Vector3 direction = target.position - transform.position;

        targetRotation = Quaternion.LookRotation(direction);

        //Debug.Log("Snap Start | Current: " + transform.rotation.eulerAngles + " Target: " + targetRotation.eulerAngles);

        isSnapping = true;
    }


    private void SetRotationValues()
    {
        Vector3 angles = transform.localEulerAngles;

        yaw = angles.y;

        pitch = angles.x;

        if (pitch > 180)
        {
            pitch -= 360;
        }
    }
}