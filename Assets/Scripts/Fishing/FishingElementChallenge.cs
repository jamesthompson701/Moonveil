using UnityEngine;

public class FishingElementChallenge : MonoBehaviour
{
    public bool completed;

    private ElementType currentRequired;

    private bool wasCorrectLastFrame;

    public float winTime = 10f;
    private float successTimer;

    public float swapGraceTime = 1.2f;
    private float graceTimer;

    private void OnEnable()
    {
        Debug.Log("FishingElementChallenge ENABLED");

        completed = false;
        successTimer = 0f;
        graceTimer = swapGraceTime;
        wasCorrectLastFrame = false;
        currentRequired = ElementType.Water;
    }

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
        Debug.Log("Required: " + currentRequired + " | Choice: " + SpellManager2.Instance.attackChoice + " | Correct: " + correct);

        if (correct)
        {
            successTimer += Time.deltaTime;
            Debug.Log("Timer: " + successTimer + " | Object: " + gameObject.name + " | Frame: " + Time.frameCount);

            Debug.Log("Success Timer = " + successTimer);

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

        // why is this just in update? -> wasCorrectLastFrame = correct;

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
        {
            return false;
        }

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
    public void SetRequired(ElementType newElement)
    {
        currentRequired = newElement;

        FishingManager.Instance.SetRequiredElementUI(newElement);

        graceTimer = swapGraceTime;

        wasCorrectLastFrame = false;

        Debug.Log("New required element: " + newElement);
    }
}