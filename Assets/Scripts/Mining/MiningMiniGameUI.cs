using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningMiniGameUI : MonoBehaviour
{
    public List<Button> buttons;

    private List<int> sequence = new List<int>();
    private int currentIndex = 0;

    private MiningManager manager;

    void Start()
    {
        manager = FindObjectOfType<MiningManager>();

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }

        gameObject.SetActive(false);
    }

    public void StartMiniGame(RockState state)
    {
        gameObject.SetActive(true);

        sequence.Clear();
        currentIndex = 0;

        int length = 3;

        if (state == RockState.Fresh)
            length = 5;
        else if (state == RockState.Cracked)
            length = 3;

        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, buttons.Count));
        }

        StartCoroutine(ShowSequence());
    }

    IEnumerator ShowSequence()
    {
        foreach (int index in sequence)
        {
            buttons[index].image.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            buttons[index].image.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnButtonPressed(int index)
    {
        if (sequence[currentIndex] == index)
        {
            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                gameObject.SetActive(false);
                manager.EndMining(true);
            }
        }
        else
        {
            gameObject.SetActive(false);
            manager.EndMining(false);
        }
    }
}