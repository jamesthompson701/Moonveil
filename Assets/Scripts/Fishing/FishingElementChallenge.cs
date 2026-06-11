using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
FishingFish currentFish;

    int currentIndex;

    public bool completed;

    public void StartChallenge(FishingFish fish)
    {
        currentFish = fish;

        currentIndex = 0;

        completed = false;

        StartCoroutine(RunChallenge());
    }

    IEnumerator RunChallenge()
    {
        while(currentIndex < currentFish.elementSequence.Length)
        {
            ElementType required = currentFish.elementSequence[currentIndex];

            float timer = currentFish.switchTime;

            while(timer > 0)
            {
                timer -= Time.deltaTime;

                if(PlayerHasCorrectElement(required))
                {
                    currentIndex++;
                    break;
                }

                yield return null;
            }

            if(timer <= 0)
            {
                yield break;
            }
        }

        completed = true;
    }

    bool PlayerHasCorrectElement(ElementType element)
    {
        return true;
    }
}
