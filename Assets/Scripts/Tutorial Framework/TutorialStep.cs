using UnityEngine;
using PixelCrushers.DialogueSystem;

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
    }

    public enum ActivationMode
    {
        Immediate,
        Proximity,
        Interactable
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

    [Header("Proximity Settings")]
    public Transform player;
    public Transform proximityTarget;
    public float proximityDistance = 5f;

    [Header("Interactable Settings")]
    public Interactable interactableTarget;

    [Header("Dialogue Behavior")]
    public bool closeDialogueOnComplete = true;

    private bool hasStarted;
    private bool hasCompleted;
    private bool listeningForTutorialEvent;
    private bool listeningForInteractableActivation;
    private bool listeningForInteractableCompletion;

    private void OnEnable()
    {
        Debug.Log($"{name} TutorialStep OnEnable. Activation: {activationMode}, Start Conversation: {startConversation}");
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
    }

    private void BeginStep()
    {
        if (hasStarted || hasCompleted) return;

        hasStarted = true;

        if (!string.IsNullOrEmpty(startConversation))
        {
            DialogueManager.StartConversation(startConversation);
        }

        SubscribeToTutorialEvent();
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

        listeningForTutorialEvent = false;
    }

    private void CompleteStep()
    {
        if (hasCompleted) return;

        hasCompleted = true;

        GameObject next = nextTutorialStep;

        UnsubscribeFromTutorialEvent();
        UnsubscribeFromInteractableActivation();
        UnsubscribeFromInteractableCompletion();

        if (closeDialogueOnComplete && DialogueManager.IsConversationActive)
        {
            DialogueManager.StopConversation();
        }

        if (!string.IsNullOrEmpty(completeConversation))
        {
            DialogueManager.StartConversation(completeConversation);
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