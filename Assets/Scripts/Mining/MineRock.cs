using System.Collections;
using UnityEngine;

public enum MineralType
{
    Fire, Water, Air
}

public class MineRock : MonoBehaviour
{
    [Header("Gem Visual")]
    public Renderer[] gemRenderers;

    public Material fireMaterial;
    public Material waterMaterial;
    public Material airMaterial;

    [Header("Reward")]
    public ItemSO rewardGem;

    [Header("Timers")]
    public float activeTime = 8f;
    public float respawnTime = 300f;
    private Coroutine activeTimerRoutine;

    private MineralType requiredType;

    private bool raised = false;
    private bool onCooldown = false;

    private Vector3 buriedPosition;
    private Vector3 raisedPosition;

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

    void Start()
    {
        buriedPosition = transform.position;

        raisedPosition = buriedPosition + Vector3.up * 5f;

        audioSource = GetComponent<AudioSource>();

        transform.position = buriedPosition;

        SetGemVisible(false);

        PlayFX(readyFX);

        //Debug.Log(name + " gem count = " + gemRenderers.Length);
    }

    void SetGemMaterial(Material mat)
    {
        if (gemRenderers == null)
            return;

        foreach (Renderer r in gemRenderers)
        {
            r.material = mat;
        }
    }

    void SetGemVisible(bool visible)
    {
        if (gemRenderers == null)
        {
            return;
        }

        foreach (Renderer r in gemRenderers)
        {
            r.enabled = visible;
        }
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

        StopFX(readyFX);

        raised = true;

        SetGemVisible(true);

        transform.position = raisedPosition;

        PlayFX(raiseFX);

        requiredType = (MineralType)Random.Range(0, 3);

        switch (requiredType)
        {
            case MineralType.Fire:
                {
                    SetGemMaterial(fireMaterial);
                    break;
                }

            case MineralType.Water:
                {
                    SetGemMaterial(waterMaterial);
                    break;
                }

            case MineralType.Air:
                {
                    SetGemMaterial(airMaterial);
                    break;
                }
        }

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

        //Debug.Log(name + " raised: " + raisedPosition);
    }

    void CheckSpell(MineralType spellType)
    {
        if (!raised)
            return;

        if (spellType == requiredType)
        {
            Success();
        }
    }

    void Success()
    {
        Debug.Log("Correct Element");

        PlayFX(successFX);

        InventoryManager.instance.invSO.AddItem(rewardGem, 1);

        if (audioSource && successSound)
        {
            audioSource.PlayOneShot(successSound);
            Debug.Log("successSound played");
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

        SetGemVisible(false);

        if (audioSource && sinkSound)
        {
            audioSource.PlayOneShot(sinkSound);
            Debug.Log("sinkSound played");
        }

        yield return StartCoroutine(SinkRock());

        yield return new WaitForSeconds(respawnTime);

        onCooldown = false;

        PlayFX(readyFX);
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

    void PlayFX(ParticleSystem[] effects)
    {
        if (effects == null)
            return;

        foreach (ParticleSystem fx in effects)
        {
            if (fx != null)
            {
                fx.Play();
            }
        }
    }

    void StopFX(ParticleSystem[] effects)
    {
        if (effects == null)
            return;

        foreach (ParticleSystem fx in effects)
        {
            if (fx != null)
            {
                fx.Stop();
            }
        }
    }
}