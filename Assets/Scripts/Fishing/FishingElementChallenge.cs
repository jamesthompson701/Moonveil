using UnityEngine;

public class FishingElementChallenge : MonoBehaviour
{
    public bool completed;

    private ElementType currentRequired;

    private bool wasCorrectLastFrame;

    public float winTime = 10f;
    private float successTimer;

    public float swapGraceTime = 1.2f;
    public float startGraceTime = 4f;
    private float graceTimer;

    public float Progress
    {
        get
        {
            return successTimer / winTime;
        }
    }

    private void OnEnable()
    {
        //Debug.Log("FishingElementChallenge ENABLED");

        completed = false;
        successTimer = 0f;
        graceTimer = startGraceTime;
        wasCorrectLastFrame = false;
        currentRequired = ElementType.Fire;
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
        //Debug.Log("Required: " + currentRequired + " | Choice: " + SpellManager2.Instance.attackChoice + " | Correct: " + correct);

        //Debug.Log("Required: " + currentRequired + " | Choice: " + SpellManager2.Instance.attackChoice + " | Correct: " + correct + " | Grace: " + graceTimer);

        if (correct)
        {
            FishingManager.Instance.HighlightNet(currentRequired);
            successTimer += Time.deltaTime;
            //Debug.Log("Timer: " + successTimer + " | Object: " + gameObject.name + " | Frame: " + Time.frameCount);

            //Debug.Log("Success Timer = " + successTimer);

            if (successTimer >= winTime)
            {
                completed = true;
                FishingManager.Instance.SuccessFishing();
                FishingManager.Instance.ClearNets();
                return;
            }

            if (!wasCorrectLastFrame)
            {
                graceTimer = swapGraceTime;
            }
        }
        if (!correct)
        {
            FishingManager.Instance.ClearNets();
        }
        if (graceTimer <= 0f)
        {
            FishingManager.Instance.FailFishing();
        }

        FishingManager.Instance.UpdateCatchProgress(Progress);
    }

    private void OnTriggerEnter(Collider other)
    {
        ElementZone zone = other.GetComponent<ElementZone>();

        if (zone != null)
        {
            SetRequired(zone.element);
            graceTimer = startGraceTime;
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

        //Debug.Log("New required element: " + newElement);
    }
}