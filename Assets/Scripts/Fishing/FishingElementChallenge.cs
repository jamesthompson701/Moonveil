using UnityEngine;

public class FishingElementChallenge : MonoBehaviour
{
    public bool completed;

    private ElementType currentRequired;

    private bool wasCorrectLastFrame;

    public float winTime = 20f;
    private float successTimer;

    public float swapGraceTime = 1.2f;
    private float graceTimer;

    void Update()
    {
        if (!FishingManager.Instance.inFishingMode || completed)
        {
            return;
        }

        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }

        bool correct = PlayerHasCorrectElement(currentRequired);

        if (correct)
        {
            successTimer += Time.deltaTime;

            if (successTimer >= winTime)
            {
                completed = true;

                FishingManager.Instance.SuccessFishing();

                return;
            }

            if (!wasCorrectLastFrame)
            {
                graceTimer = swapGraceTime;
            }
        }

        wasCorrectLastFrame = correct;

        if (graceTimer <= 0f)
        {
            FishingManager.Instance.FailFishing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ElementZone zone = other.GetComponent<ElementZone>();
        if (zone != null)
        {
            SetRequired(zone.element);
        }
    }

    private bool PlayerHasCorrectElement(ElementType required)
    {
        if (SpellManager2.Instance == null)
            return false;

        int choice = SpellManager2.Instance.attackChoice;

        return required switch
        {
            ElementType.Fire => choice == 1,
            ElementType.Earth => choice == 2,
            ElementType.Water => choice == 3,
            ElementType.Air => choice == 4,
            _ => false
        };
    }
    private void SetRequired(ElementType newElement)
    {
        currentRequired = newElement;

        FishingManager.Instance.SetRequiredElementUI(newElement);

        graceTimer = swapGraceTime;

        wasCorrectLastFrame = false;

        Debug.Log("New required element: " + newElement);
    }
}