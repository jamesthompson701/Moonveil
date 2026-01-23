using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    public bool isOnGround;
    private Vector3 move;
    private Vector3 playerVelocity;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    //[SerializeField] private float interpolationSpeed = 8f;
    [SerializeField] private float speedIncrease = 2f;
    [SerializeField] private float speedMin = 5f;
    [SerializeField] private float speedMax = 15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private float gravityValue = -9.81f;

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        float jump = Input.GetAxisRaw("Jump");

        isOnGround = controller.isGrounded;

        if (isOnGround && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 input = new Vector3(xInput, 0, yInput).normalized;

        if (cameraTransform != null)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            move = (cameraRight * input.x + cameraForward * input.z);
        }
        else
        {
            move = input;
        }

        if (move.sqrMagnitude > 0.001f)
        {
            move.Normalize();

            speed += speedIncrease * Time.deltaTime;
        }

        //This is supposed to make the player jump while retaining any momentum gained
        if (jump == 1 && isOnGround)
        {
            playerVelocity.y = jumpSpeed;
        }

        //Applying Gravity to the Jump
        playerVelocity.y += gravityValue * Time.deltaTime;

        speed = Mathf.Clamp(speed, speedMin, speedMax);

        controller.Move((move * speed * Time.deltaTime) + (playerVelocity.y * Vector3.up * Time.deltaTime));
    }
}
