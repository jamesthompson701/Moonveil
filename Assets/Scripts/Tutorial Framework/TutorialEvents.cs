using System;
using UnityEngine;

/*
HOW TO ADD A NEW TUTORIAL EVENT

1. Add the event below:
    public static event Action MyEvent;

2. Add a trigger:
    public static void TriggerMyEvent()
    {
        MyEvent?.Invoke();
    }

3. Add MyEvent to TutorialStep's enum.

4. Subscribe/Unsubscribe inside TutorialStep.

5. Call TutorialEvents.TriggerMyEvent()
   from your gameplay script when the action occurs.
*/
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
    public static event Action CompleteQuest;
    public static event Action FireSpell; 
    public static event Action EarthSpell; 
    public static event Action WaterSpell; 
    public static event Action AirSpell;
    public static event Action HitBush;

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

    public static void TriggerCompleteQuest()
    {
        Debug.Log("Complete Quest Event");
        CompleteQuest?.Invoke();
    }

    public static void TriggerFireSpell()
    {
        FireSpell?.Invoke();
    }

    public static void TriggerEarthSpell()
    {
        EarthSpell?.Invoke();
    }

    public static void TriggerWaterSpell()
    {
        WaterSpell?.Invoke();
    }

    public static void TriggerAirSpell()
    {
        AirSpell?.Invoke();
    }

    public static void TriggerHitBush() => HitBush?.Invoke();
}