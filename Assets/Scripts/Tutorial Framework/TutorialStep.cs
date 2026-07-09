using UnityEngine;
using PixelCrushers.DialogueSystem;

/*
=========================
HOW TO ADD A NEW EVENT
=========================

1. Add it to TutorialEventType.

2. In SubscribeToTutorialEvent():

    case TutorialEventType.MyEvent:
        TutorialEvents.MyEvent += CompleteStep;
        break;

3. In UnsubscribeFromTutorialEvent():

    TutorialEvents.MyEvent -= CompleteStep;

4. Trigger it from either:
   - TutorialInputEventBroadcaster (for player input)
   - Your gameplay script (for game events)

*/
public class TutorialStep : MonoBehaviour
{
    public enum TutorialEventType
    {
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        Jump,
        Sprint,
        Fly,
        Look,
        Interact,
        Till,
        Plant,
        Water,
        Harvest
        FireSpell,
        EarthSpell,
        WaterSpell,
        AirSpell
    }

    public enum ActivationMode
    {
        Immediate,
        Proximity,
        Interactable,
        QuestCompletion
    }

    [Header("Activation")]
    public ActivationMode activationMode = ActivationMode.Immediate;

    [Header("Tutorial Event To Listen For")]
    public TutorialEventType requiredEvent;

    [Header("Pixel Crushers Conversations")]
    public string startConversation;
    public string completeConversation;

    [Header("Next Step")]
    public GameObject nextTutorialStep;

    // added for minigame pop-up canvases
    [Header("Tutorial Popup")]
    public GameObject tutorialPopup;

    [Header("Proximity Settings")]
    public Transform player;
    public Transform proximityTarget;
    public float proximityDistance = 5f;

    [Header("Interactable Settings")]
    public Interactable interactableTarget;

    [Header("Dialogue Behavior")]
    public bool closeDialogueOnComplete = true;

    private static TutorialStep activeInstruction;
    private bool hasStarted;
    private bool hasCompleted;
    private bool listeningForTutorialEvent;
    private bool listeningForInteractableActivation;
    private bool listeningForQuestActivation;
    private bool listeningForInteractableCompletion;

    private void OnEnable()
    {
        hasStarted = false;
        hasCompleted = false;
        listeningForTutorialEvent = false;
        listeningForInteractableActivation = false;
        listeningForInteractableCompletion = false;

        if (activationMode == ActivationMode.Immediate)
        {
            BeginStep();
        }
        else if (activationMode == ActivationMode.Interactable)
        {
            SubscribeToInteractableActivation();
        }
        else if (activationMode == ActivationMode.QuestCompletion)
        {
            SubscribeToQuestActivation();
        }
    }

    private void Update()
    {
        if (hasStarted || hasCompleted) return;

        if (activationMode != ActivationMode.Proximity) return;

        if (player == null || proximityTarget == null) return;

        float distance = Vector3.Distance(player.position, proximityTarget.position);

        if (distance <= proximityDistance)
        {
            BeginStep();
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromTutorialEvent();
        UnsubscribeFromInteractableActivation();
        UnsubscribeFromInteractableCompletion();
        UnsubscribeFromQuestActivation();
    }

    private void BeginStep()
    {
        if (hasStarted || hasCompleted) return;

        if (activeInstruction != null && activeInstruction != this)
        {
            return;
        }

        activeInstruction = this;
        hasStarted = true;

        // for added tutorial canvas functionality
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(true);
        }

        if (!string.IsNullOrEmpty(startConversation))
        {
            DialogueManager.StartConversation(startConversation);
            Invoke(nameof(SubscribeToTutorialEvent), 0.5f);
        }
        else
        {
            SubscribeToTutorialEvent();
        }
    }

    private void SubscribeToInteractableActivation()
    {
        if (listeningForInteractableActivation) return;

        listeningForInteractableActivation = true;
        Interactable.OnAnyInteract += OnInteractableUsedForActivation;
    }

    private void UnsubscribeFromInteractableActivation()
    {
        if (!listeningForInteractableActivation) return;

        Interactable.OnAnyInteract -= OnInteractableUsedForActivation;
        listeningForInteractableActivation = false;
    }

    private void SubscribeToQuestActivation()
    {
        if (listeningForQuestActivation) return;
        Debug.Log("Listening for complete quest event");
        listeningForQuestActivation = true;
        TutorialEvents.CompleteQuest += BeginStep;
        TutorialEvents.CompleteQuest += UnsubscribeFromQuestActivation;
    }

    private void UnsubscribeFromQuestActivation()
    {
        if (!listeningForQuestActivation) return;

        TutorialEvents.CompleteQuest -= BeginStep;
        listeningForQuestActivation = false;
    }

    private void SubscribeToInteractableCompletion()
    {
        if (listeningForInteractableCompletion) return;

        listeningForInteractableCompletion = true;
        Interactable.OnAnyInteract += OnInteractableUsedForCompletion;
    }

    private void UnsubscribeFromInteractableCompletion()
    {
        if (!listeningForInteractableCompletion) return;

        Interactable.OnAnyInteract -= OnInteractableUsedForCompletion;
        listeningForInteractableCompletion = false;
    }

    private void OnInteractableUsedForActivation(Interactable interactedObject)
    {
        if (Time.timeScale == 0f) return;

        if (hasStarted || hasCompleted) return;

        if (interactableTarget != null && interactedObject != interactableTarget)
        {
            return;
        }

        UnsubscribeFromInteractableActivation();

        BeginStep();

        if (requiredEvent == TutorialEventType.Interact)
        {
            CompleteStep();
        }
    }

    private void OnInteractableUsedForCompletion(Interactable interactedObject)
    {
        if (Time.timeScale == 0f) return;

        if (!hasStarted || hasCompleted) return;

        if (interactableTarget != null && interactedObject != interactableTarget)
        {
            return;
        }

        CompleteStep();
    }

    private void SubscribeToTutorialEvent()
    {
        if (listeningForTutorialEvent) return;

        listeningForTutorialEvent = true;

        switch (requiredEvent)
        {
            case TutorialEventType.MoveForward:
                TutorialEvents.MoveForward += CompleteStep;
                break;
            case TutorialEventType.MoveBackward:
                TutorialEvents.MoveBackward += CompleteStep;
                break;
            case TutorialEventType.MoveLeft:
                TutorialEvents.MoveLeft += CompleteStep;
                break;
            case TutorialEventType.MoveRight:
                TutorialEvents.MoveRight += CompleteStep;
                break;
            case TutorialEventType.Jump:
                TutorialEvents.Jump += CompleteStep;
                break;
            case TutorialEventType.Sprint:
                TutorialEvents.Sprint += CompleteStep;
                break;
            case TutorialEventType.Fly:
                TutorialEvents.Fly += CompleteStep;
                break;
            case TutorialEventType.Look:
                TutorialEvents.Look += CompleteStep;
                break;
            case TutorialEventType.Interact:
                if (interactableTarget != null)
                {
                    SubscribeToInteractableCompletion();
                }
                else
                {
                    TutorialEvents.Interact += CompleteStep;
                }
                break;
            case TutorialEventType.Till:
                TutorialEvents.Till += CompleteStep;
                break;
            case TutorialEventType.Plant:
                TutorialEvents.Plant += CompleteStep;
                break;
            case TutorialEventType.Water:
                TutorialEvents.Water += CompleteStep;
                break;
            case TutorialEventType.Harvest:
                TutorialEvents.Harvest += CompleteStep;
                break;
            case TutorialEventType.FireSpell:
                TutorialEvents.FireSpell += CompleteStep;
                break;

            case TutorialEventType.EarthSpell:
                TutorialEvents.EarthSpell += CompleteStep;
                break;

            case TutorialEventType.WaterSpell:
                TutorialEvents.WaterSpell += CompleteStep;
                break;

            case TutorialEventType.AirSpell:
                TutorialEvents.AirSpell += CompleteStep;
                break;
        }
    }

    private void UnsubscribeFromTutorialEvent()
    {
        if (!listeningForTutorialEvent) return;

        TutorialEvents.MoveForward -= CompleteStep;
        TutorialEvents.MoveBackward -= CompleteStep;
        TutorialEvents.MoveLeft -= CompleteStep;
        TutorialEvents.MoveRight -= CompleteStep;
        TutorialEvents.Jump -= CompleteStep;
        TutorialEvents.Sprint -= CompleteStep;
        TutorialEvents.Fly -= CompleteStep;
        TutorialEvents.Look -= CompleteStep;
        TutorialEvents.Interact -= CompleteStep;
        TutorialEvents.Till -= CompleteStep;
        TutorialEvents.Plant -= CompleteStep;
        TutorialEvents.Water -= CompleteStep;
        TutorialEvents.Harvest -= CompleteStep;
        TutorialEvents.FireSpell -= CompleteStep;
        TutorialEvents.EarthSpell -= CompleteStep;
        TutorialEvents.WaterSpell -= CompleteStep;
        TutorialEvents.AirSpell -= CompleteStep;

        listeningForTutorialEvent = false;
    }

    private void CompleteStep()
    {
        if (Time.timeScale == 0f) return;

        if (hasCompleted) return;

        hasCompleted = true;

        GameObject next = nextTutorialStep;

        UnsubscribeFromTutorialEvent();
        UnsubscribeFromInteractableActivation();
        UnsubscribeFromInteractableCompletion();
        UnsubscribeFromQuestActivation();

        if (closeDialogueOnComplete && DialogueManager.IsConversationActive)
        {
            DialogueManager.StopConversation();
        }

        if (!string.IsNullOrEmpty(completeConversation))
        {
            DialogueManager.StartConversation(completeConversation);
        }

        if (activeInstruction == this)
        {
            activeInstruction = null;
        }
        // last bit of tutorial addition
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }

        gameObject.SetActive(false);

        if (next != null)
        {
            next.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (activationMode != ActivationMode.Proximity) return;
        if (proximityTarget == null) return;

        Gizmos.DrawWireSphere(proximityTarget.position, proximityDistance);
    }
}