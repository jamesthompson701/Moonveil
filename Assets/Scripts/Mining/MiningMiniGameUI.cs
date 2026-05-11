using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningMiniGameUI : MonoBehaviour
{
    private bool canInput = false;
    public List<Button> buttons;

    private List<int> sequence = new List<int>();
    private int currentIndex = 0;

    private MiningManager manager;

    void SetButtonsInteractable(bool active)
    {
        foreach (Button button in buttons)
        {
            button.interactable = active;
        }
    }

    void Start()
    {
        manager = FindFirstObjectByType<MiningManager>();

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }

        gameObject.SetActive(false);
    }

    public void StartMiniGame(RockState state)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);
        canInput = false;


        sequence.Clear();
        currentIndex = 0;

        int length = 3;
        float showSpeed = 0.5f;

        if (state == RockState.Fresh)
        {
            length = 5;
            showSpeed = 0.4f;
        }
        else if (state == RockState.Cracked)
        {
            length = 3;
            showSpeed = 0.6f;
        }

        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, buttons.Count));
        }

        StartCoroutine(StartWithDelay(showSpeed));
    }

    IEnumerator StartWithDelay(float speed)
    {
        canInput = false;

        yield return new WaitForSeconds(1.5f); // buffer before showing

        yield return StartCoroutine(ShowSequence(speed));
    }

    IEnumerator ShowSequence(float speed)
    {
        canInput = false;
        SetButtonsInteractable(false);

        foreach (int index in sequence)
        {
            buttons[index].image.color = Color.yellow;
            yield return new WaitForSeconds(speed);

            buttons[index].image.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        currentIndex = 0;

        SetButtonsInteractable(true);
        canInput = true;

        Debug.Log("PLAYER INPUT ENABLED");
    }


    void OnButtonPressed(int index)
    {
        if (!canInput || !gameObject.activeInHierarchy || sequence == null || sequence.Count == 0)
            return;

        Debug.Log("BUTTON PRESSED: " + index);

        Debug.Log("Current Index Before: " + currentIndex);
        Debug.Log("Expected: " + sequence[currentIndex]);

        if (sequence[currentIndex] == index)
        {
            currentIndex++;

            Debug.Log("Correct");
            Debug.Log("Current Index After: " + currentIndex);

            if (currentIndex >= sequence.Count)
            {
                if (!canInput) return;

                canInput = false;
                SetButtonsInteractable(false);
                StopAllCoroutines();

                Debug.Log("MINING WON");

                gameObject.SetActive(false);
                manager.EndMining(true);

                return;
            }
        }
        else
        {
            Debug.Log("WRONG BUTTON");

            canInput = false;
            SetButtonsInteractable(false);

            gameObject.SetActive(false);
            manager.EndMining(false);
        }
    }
}