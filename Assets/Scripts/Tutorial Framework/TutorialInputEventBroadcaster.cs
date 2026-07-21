using UnityEngine;

/*
This script is ONLY for generic player inputs
(move, jump, sprint, look, interact, etc.)

If your tutorial event happens because gameplay succeeded
(harvesting, fishing, crafting, quest complete, etc.)
DO NOT ADD IT HERE.

Instead, call TutorialEvents.TriggerYourEvent()
from the gameplay script that already knows it happened.
*/
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
    private bool tilled;
    private bool planted;
    private bool watered;
    private bool harvested;
    private bool questCompleted;
    private bool hitBush;

    public GameObject afterMiningQuest;
    public bool afterMiningQuestActivated = false;

    public GameObject afterCombatQuest;
    public bool afterCombatQuestComplete = false;


    private void Update()
    {
        CheckMovementInput();
        CheckButtonInput();
        CheckMouseLookInput();
        CheckIfTilled();
        CheckIfPlanted();
        CheckIfWatered();
        CheckIfHarvested();
        CheckIfHitBush();
    }

    private void CheckMovementInput()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);

        float resetThreshold = movementThreshold * 0.5f;

        if (Mathf.Abs(vertical) < resetThreshold)
        {
            hasMovedForward = false;
            hasMovedBackward = false;
        }

        if (Mathf.Abs(horizontal) < resetThreshold)
        {
            hasMovedLeft = false;
            hasMovedRight = false;
        }

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
        if (Input.GetButtonDown(jumpButton))
        {
            TutorialEvents.TriggerJump();
        }

        if (Input.GetButtonDown(sprintButton))
        {
            TutorialEvents.TriggerSprint();
        }

        if (Input.GetButtonDown(flyButton))
        {
            TutorialEvents.TriggerFly();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TutorialEvents.TriggerInteract();
        }
    }

    private void CheckMouseLookInput()
    {
        float mouseX = Mathf.Abs(Input.GetAxisRaw(mouseXAxis));
        float mouseY = Mathf.Abs(Input.GetAxisRaw(mouseYAxis));

        if (mouseX < mouseLookThreshold * 0.5f && mouseY < mouseLookThreshold * 0.5f)
        {
            hasLooked = false;
        }

        if (!hasLooked && (mouseX > mouseLookThreshold || mouseY > mouseLookThreshold))
        {
            hasLooked = true;
            TutorialEvents.TriggerLook();
        }
    }

    private void CheckIfTilled()
    {
        if (!tilled && TimeManager.instance.tilledDone == true)
        {
            tilled = true;
            //TutorialEvents.TriggerTill();
        }
    }
    private void CheckIfPlanted()
    {
        if (!planted && TimeManager.instance.plantDone == true)
        {
            planted = true;
            TutorialEvents.TriggerPlant();
        }
    }
    private void CheckIfWatered()
    {
        if (!watered && TimeManager.instance.waterDone == true)
        {
            watered = true;
            TutorialEvents.TriggerWater();
        }
    }
    private void CheckIfHarvested()
    {
        if (!harvested && TimeManager.instance.harvestDone == true)
        {
            harvested = true;
            TutorialEvents.TriggerHarvest();
        }
    }
    private void CheckIfHitBush()
    {
        if (!hitBush && TimeManager.instance.hitBushDone == true)
        {
            harvested = true;
            TutorialEvents.TriggerHarvest();
        }
    }


    // These public methods let teammates trigger tutorial events from their own systems
    // without needing to touch the event code directly.
    public void ManuallyTriggerInteract()
    {
        TutorialEvents.TriggerInteract();
    }

    public void ManuallyTriggerFly()
    {
        TutorialEvents.TriggerFly();
    }
}