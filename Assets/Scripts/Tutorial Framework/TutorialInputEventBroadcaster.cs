using UnityEngine;

public class TutorialInputEventBroadcaster : MonoBehaviour
{
    [Header("Movement Axis Names")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string mouseXAxis = "Mouse X";
    public string mouseYAxis = "Mouse Y";

    [Header("Button Names")]
    public string jumpButton = "Jump";
    public string sprintButton = "Sprint";
    public string flyButton = "Fly";
    public string interactButton = "Interact";

    [Header("Sensitivity")]
    public float movementThreshold = 0.2f;
    public float mouseLookThreshold = 0.1f;

    private bool hasMovedForward;
    private bool hasMovedBackward;
    private bool hasMovedLeft;
    private bool hasMovedRight;
    private bool hasJumped;
    private bool hasSprinted;
    private bool hasFlown;
    private bool hasLooked;
    private bool hasInteracted;

    private void Update()
    {
        CheckMovementInput();
        CheckButtonInput();
        CheckMouseLookInput();
    }

    private void CheckMovementInput()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);

        if (!hasMovedForward && vertical > movementThreshold)
        {
            hasMovedForward = true;
            TutorialEvents.TriggerMoveForward();
        }

        if (!hasMovedBackward && vertical < -movementThreshold)
        {
            hasMovedBackward = true;
            TutorialEvents.TriggerMoveBackward();
        }

        if (!hasMovedLeft && horizontal < -movementThreshold)
        {
            hasMovedLeft = true;
            TutorialEvents.TriggerMoveLeft();
        }

        if (!hasMovedRight && horizontal > movementThreshold)
        {
            hasMovedRight = true;
            TutorialEvents.TriggerMoveRight();
        }
    }

    private void CheckButtonInput()
    {
        if (!hasJumped && Input.GetButtonDown(jumpButton))
        {
            hasJumped = true;
            TutorialEvents.TriggerJump();
        }

        if (!hasSprinted && Input.GetButtonDown(sprintButton))
        {
            hasSprinted = true;
            TutorialEvents.TriggerSprint();
        }

        if (!hasFlown && Input.GetButtonDown(flyButton))
        {
            hasFlown = true;
            TutorialEvents.TriggerFly();
        }

        if (!hasInteracted && Input.GetButtonDown(interactButton))
        {
            hasInteracted = true;
            TutorialEvents.TriggerInteract();
        }
    }

    private void CheckMouseLookInput()
    {
        float mouseX = Mathf.Abs(Input.GetAxisRaw(mouseXAxis));
        float mouseY = Mathf.Abs(Input.GetAxisRaw(mouseYAxis));

        if (!hasLooked && (mouseX > mouseLookThreshold || mouseY > mouseLookThreshold))
        {
            hasLooked = true;
            TutorialEvents.TriggerLook();
        }
    }

    // These public methods let teammates trigger tutorial events from their own systems
    // without needing to touch the event code directly.

    public void ManuallyTriggerInteract()
    {
        if (hasInteracted) return;

        hasInteracted = true;
        TutorialEvents.TriggerInteract();
    }

    public void ManuallyTriggerFly()
    {
        if (hasFlown) return;

        hasFlown = true;
        TutorialEvents.TriggerFly();
    }
}