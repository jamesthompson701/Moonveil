using System;

public static class TutorialEvents
{
    public static event Action MoveForward;
    public static event Action MoveBackward;
    public static event Action MoveLeft;
    public static event Action MoveRight;
    public static event Action Jump;
    public static event Action Sprint;
    public static event Action Fly;
    public static event Action Look;
    public static event Action Interact;
    public static event Action Till;
    public static event Action Plant;
    public static event Action Water;
    public static event Action Harvest;

    public static void TriggerMoveForward() => MoveForward?.Invoke();
    public static void TriggerMoveBackward() => MoveBackward?.Invoke();
    public static void TriggerMoveLeft() => MoveLeft?.Invoke();
    public static void TriggerMoveRight() => MoveRight?.Invoke();
    public static void TriggerJump() => Jump?.Invoke();
    public static void TriggerSprint() => Sprint?.Invoke();
    public static void TriggerFly() => Fly?.Invoke();
    public static void TriggerLook() => Look?.Invoke();
    public static void TriggerInteract() => Interact?.Invoke();
    public static void TriggerTill() => Till?.Invoke();
    public static void TriggerPlant() => Plant?.Invoke();
    public static void TriggerWater() => Water?.Invoke();
    public static void TriggerHarvest() => Harvest?.Invoke();
}