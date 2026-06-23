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
        Interact
    }

    [Header("Tutorial Event To Listen For")]
    public TutorialEventType requiredEvent;

    [Header("Pixel Crushers Conversations")]
    public string startConversation;
    public string completeConversation;

    [Header("Next Step")]
    public GameObject nextTutorialStep;

    private bool hasCompleted;

    private void OnEnable()
    {
        SubscribeToEvent();

        if (!string.IsNullOrEmpty(startConversation))
        {
            DialogueManager.StartConversation(startConversation);
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromEvent();
    }

    private void SubscribeToEvent()
    {
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
                TutorialEvents.Interact += CompleteStep;
                break;
        }
    }

    private void UnsubscribeFromEvent()
    {
        TutorialEvents.MoveForward -= CompleteStep;
        TutorialEvents.MoveBackward -= CompleteStep;
        TutorialEvents.MoveLeft -= CompleteStep;
        TutorialEvents.MoveRight -= CompleteStep;
        TutorialEvents.Jump -= CompleteStep;
        TutorialEvents.Sprint -= CompleteStep;
        TutorialEvents.Fly -= CompleteStep;
        TutorialEvents.Look -= CompleteStep;
        TutorialEvents.Interact -= CompleteStep;
    }

    private void CompleteStep()
    {
        if (hasCompleted) return;

        hasCompleted = true;
        DialogueManager.Instance.standardDialogueUI.OnContinueConversation();
        UnsubscribeFromEvent();

        if (!string.IsNullOrEmpty(completeConversation))
        {
            DialogueManager.StartConversation(completeConversation);
        }

        if (nextTutorialStep != null)
        {
            nextTutorialStep.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}