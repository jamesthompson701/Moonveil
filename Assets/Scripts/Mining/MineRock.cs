using System.Collections;
using UnityEngine;

public enum MineralType
{
    Fire, Water, Air
}

public class MineRock : MonoBehaviour
{
    [Header("Gem Models")]
    public GameObject fireGems;
    public GameObject waterGems;
    public GameObject airGems;

    [Header("Reward")]
    public ItemSO fireReward;
    public ItemSO waterReward;
    public ItemSO airReward;

    [Header("Timers")]
    public float activeTime = 8f;
    public float respawnTime = 300f;
    private Coroutine activeTimerRoutine;

    private MineralType requiredType;

    private bool raised = false;
    private bool onCooldown = false;

    private Vector3 buriedPosition;
    private Vector3 raisedPosition;

    [SerializeField] float raiseDistance = 5f;

    [Header("Push")]
    public Collider[] pushColliders;
    [SerializeField] private float pushDuration = 0.5f;

    [Header("Tutorial")]
    [SerializeField] private bool destroyOnSuccess = false;

    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip raiseSound;
    public AudioClip successSound;
    public AudioClip sinkSound;

    [Header("VFX")]
    public ParticleSystem[] raiseFX;
    public ParticleSystem[] successFX;
    public ParticleSystem[] failFX;
    public ParticleSystem[] readyFX;

    TutorialInputEventBroadcaster tutorialEvent;

    void Start()
    {
        buriedPosition = transform.position;

        raisedPosition = buriedPosition + transform.up * raiseDistance;

        audioSource = GetComponent<AudioSource>();

        transform.position = buriedPosition;

        HideAllGems();

        ShowReadyFX();

        tutorialEvent = tutorialEvent != null ? tutorialEvent : FindFirstObjectByType<TutorialInputEventBroadcaster>();

        foreach (Collider col in pushColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        //Debug.Log(name + " gem count = " + gemRenderers.Length);
    }

    void ShowGemType(MineralType type)
    {
        fireGems.SetActive(type == MineralType.Fire);
        waterGems.SetActive(type == MineralType.Water);
        airGems.SetActive(type == MineralType.Air);
    }

    public void Interact()
    {
        Debug.Log("Use Earth Spell");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown)
        {
            return;
        }

        // earth spell
        if (other.CompareTag("TillSpell"))
        {
            if (!raised)
            {
                RaiseCrystal();
            }
        }

        // fire spell
        if (other.CompareTag("FireSpell"))
        {
            CheckSpell(MineralType.Fire);
        }

        // water spell
        if (other.CompareTag("WateringSpell"))
        {
            CheckSpell(MineralType.Water);
        }

        // air spell
        if (other.CompareTag("HarvestSpell"))
        {
            CheckSpell(MineralType.Air);
        }
    }

    void RaiseCrystal()
    {
        //Debug.Log(name + " buried: " + buriedPosition);

        HideReadyFX();

        raised = true;

        transform.position = raisedPosition;

        PlayFX(raiseFX);

        if (destroyOnSuccess)
        {
            fireGems.SetActive(true);
            waterGems.SetActive(true);
            airGems.SetActive(true);

            if (audioSource && raiseSound)
            {
                audioSource.PlayOneShot(raiseSound);
                Debug.Log("raiseSound played");
            }

            if (activeTimerRoutine != null)
            {
                StopCoroutine(activeTimerRoutine);
            }

            activeTimerRoutine = StartCoroutine(ActiveTimer());

            foreach (Collider col in pushColliders)
            {
                if (col != null)
                {
                    col.enabled = true;
                }
            }

            StartCoroutine(DisablePushCollider());
        }

        else
        {
            requiredType = (MineralType)Random.Range(0, 3);

            ShowGemType(requiredType);

            if (audioSource && raiseSound)
            {
                audioSource.PlayOneShot(raiseSound);
                Debug.Log("raiseSound played");
            }

            if (activeTimerRoutine != null)
            {
                StopCoroutine(activeTimerRoutine);
            }

            activeTimerRoutine = StartCoroutine(ActiveTimer());

            foreach (Collider col in pushColliders)
            {
                if (col != null)
                {
                    col.enabled = true;
                }
            }

            StartCoroutine(DisablePushCollider());
        }
    }

    void CheckSpell(MineralType spellType)
    {
        if (!raised)
            return;

        if (spellType == requiredType)
        {
            Success();
        }
        else if (destroyOnSuccess)
        {
            Success();
        }
    }

    void HideAllGems()
    {
        fireGems.SetActive(false);
        waterGems.SetActive(false);
        airGems.SetActive(false);
    }

    void Success()
    {
        Debug.Log("Correct Element");

        PlayFX(successFX);

        if (destroyOnSuccess)
        {
            InventoryManager.instance.invSO.AddItem(fireReward, 1);
            InventoryManager.instance.invSO.AddItem(waterReward, 1);
            InventoryManager.instance.invSO.AddItem(airReward, 1);
        }
        else
        {
            switch (requiredType)
            {
                case MineralType.Fire:
                    InventoryManager.instance.invSO.AddItem(fireReward, 1);
                    break;

                case MineralType.Water:
                    InventoryManager.instance.invSO.AddItem(waterReward, 1);
                    break;

                case MineralType.Air:
                    InventoryManager.instance.invSO.AddItem(airReward, 1);
                    break;
            }
        }
        /*
        if (!tutorialEvent.afterMiningQuestActivated)
        {
            tutorialEvent.afterMiningQuestActivated = true;
            tutorialEvent.afterMiningQuest.SetActive(true);
        }
        */

        if (audioSource && successSound)
        {
            audioSource.PlayOneShot(successSound);
            Debug.Log("successSound played");
        }

        if (destroyOnSuccess)
        {
            StartCoroutine(DestroyAfterSuccess());
            return;
        }

        StartCoroutine(CooldownRoutine());
    }

    void Fail()
    {
        PlayFX(failFX);

        StartCoroutine(CooldownRoutine());
    }

    IEnumerator ActiveTimer()
    {
        yield return new WaitForSeconds(activeTime);

        if (raised)
        {
            Fail();
        }
    }

    IEnumerator CooldownRoutine()
    {
        if (activeTimerRoutine != null)
        {
            StopCoroutine(activeTimerRoutine);
            activeTimerRoutine = null;
        }

        raised = false;
        onCooldown = true;

        HideAllGems();

        if (audioSource && sinkSound)
        {
            audioSource.PlayOneShot(sinkSound);
            Debug.Log("sinkSound played");
        }

        yield return StartCoroutine(SinkRock());

        foreach (Collider col in pushColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        yield return new WaitForSeconds(respawnTime);

        onCooldown = false;

        PlayFX(readyFX);
        ShowReadyFX();
    }

    IEnumerator SinkRock()
    {
        float duration = 1f;

        Vector3 startPos = transform.position;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, buriedPosition, timer / duration);

            yield return null;
        }

        transform.position = buriedPosition;
    }

    IEnumerator DisablePushCollider()
    {
        yield return new WaitForSeconds(pushDuration);

        foreach (Collider col in pushColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    IEnumerator DestroyAfterSuccess()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void PlayFX(ParticleSystem[] effects)
    {
        if (effects == null)
            return;

        foreach (ParticleSystem fx in effects)
        {
            if (fx == null)
                continue;

            fx.gameObject.SetActive(true);

            fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fx.Clear();
            fx.Simulate(0f, true, true);
            fx.Play();
        }
    }

    void StopFX(ParticleSystem[] effects)
    {
        if (effects == null)
            return;

        foreach (ParticleSystem fx in effects)
        {
            if (fx == null)
                continue;

            fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    //damn sparkle
    void ShowReadyFX()
    {
        foreach (ParticleSystem fx in readyFX)
        {
            if (fx != null)
                fx.gameObject.SetActive(true);
        }
    }

    void HideReadyFX()
    {
        foreach (ParticleSystem fx in readyFX)
        {
            if (fx != null)
                fx.gameObject.SetActive(false);
        }
    }
}