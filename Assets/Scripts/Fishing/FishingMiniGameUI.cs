using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FishingMiniGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform upperBound;  // visual max bound
    public RectTransform innerRing;   // the player-controlled ring (scale)
    public RectTransform lowerBound;  // visual min bound

    [Header("Tuning")]
    public float visualScaleMultiplier = 1f; // to map float scale -> rect scale easily

    // state
    public bool IsActiveAndPlaying { get; private set; } = false;

    // internal
    private FishData currentFish;
    private string inputName;
    private Action<bool, FishData> onComplete;

    private float innerScale = 0.5f;

    public void StartMiniGame(FishData fish, string reelInput, Action<bool, FishData> callback)
    {
        currentFish = fish;
        inputName = reelInput;
        onComplete = callback;

        // initialize scales
        innerScale = Mathf.Clamp( (currentFish.minInnerScale + currentFish.maxInnerScale) / 2f, currentFish.minInnerScale, currentFish.maxInnerScale);
        UpdateInnerVisual();

        IsActiveAndPlaying = true;
        gameObject.SetActive(true);
        StartCoroutine(MinigameLoop());
    }

    IEnumerator MinigameLoop()
    {
        float keepTimer = 0f;
        float needed = currentFish.keepDuration;

        while (IsActiveAndPlaying)
        {
            // auto expand over time
            innerScale += currentFish.baseExpansionSpeed * Time.deltaTime;
            UpdateInnerVisual();

            // input shrinks
            if (Input.GetButtonDown(inputName))
            {
                innerScale -= currentFish.shrinkAmountPerPress;
                UpdateInnerVisual();
            }

            // check fail conditions
            if (innerScale >= currentFish.maxInnerScale)
            {
                Debug.Log("Bubble pops and fish escapes");
                EndGame(false);
                yield break;
            }

            if (innerScale <= currentFish.minInnerScale)
            {
                Debug.Log("The fish slips away");
                EndGame(false);
                yield break;
            }

            // check if innerScale is inside target bounds (inside min/max)
            float lowerTarget = currentFish.minInnerScale + (currentFish.maxInnerScale - currentFish.minInnerScale) * 0.25f;
            float upperTarget = currentFish.maxInnerScale - (currentFish.maxInnerScale - currentFish.minInnerScale) * 0.25f;

            bool inside = innerScale > lowerTarget && innerScale < upperTarget;
            if (inside)
            {
                keepTimer += Time.deltaTime;
                // optionally show progress bar
                if (keepTimer >= needed)
                {
                    Debug.Log("You caught a fish!");
                    EndGame(true);
                    yield break;
                }
            }
            else
            {
                // reset progress if leaves target zone
                keepTimer = Mathf.Max(0f, keepTimer - Time.deltaTime * 2f);
            }

            yield return null;
        }
    }

    void UpdateInnerVisual()
    {
        //Ring UI Water
        if (innerRing != null)
        {
            float s = innerScale * visualScaleMultiplier;
            innerRing.localScale = Vector3.one * s;
        }
    }

    void EndGame(bool success)
    {
        IsActiveAndPlaying = false;
        gameObject.SetActive(false);
        onComplete?.Invoke(success, currentFish);
        onComplete = null;
        currentFish = null;
    }

    public void EndGameCleanup()
    {
        // stop any running game
        IsActiveAndPlaying = false;
        gameObject.SetActive(false);
        onComplete = null;
        currentFish = null;
    }
}