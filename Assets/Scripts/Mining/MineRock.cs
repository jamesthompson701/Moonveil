using System.Collections;
using UnityEngine;

public enum MineralType
{
    Fire,
    Water,
    Air
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

    private AudioSource audioSource;

    public AudioClip raiseSound;
    public AudioClip successSound;
    public AudioClip sinkSound;

    void Start()
    {
        buriedPosition = transform.position;

        raisedPosition = buriedPosition + Vector3.up * 5f;

        audioSource = GetComponent<AudioSource>();

        transform.position = buriedPosition;

        SetGemVisible(false);

        Debug.Log("Gem Count: " + gemRenderers.Length);
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
            return;

        foreach (Renderer r in gemRenderers)
        {
            r.enabled = visible;
        }
    }

   public void Interact()
    {
        // space for tutorial or debug
        Debug.Log("Use Earth Spell");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown)
        {
            return;
        }

        // EARTH SPELL
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
        if (other.CompareTag("WindSpell"))
        {
            CheckSpell(MineralType.Air);
        }
    }

    void RaiseCrystal()
    {
         raised = true;

        SetGemVisible(true);

        transform.position = raisedPosition;

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

        if(audioSource && raiseSound)
        {
            audioSource.PlayOneShot(raiseSound);
        }

        if (activeTimerRoutine != null)
        {
            StopCoroutine(activeTimerRoutine);
        }

        activeTimerRoutine = StartCoroutine(ActiveTimer());
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

        InventoryManager.instance.invSO.AddItem(rewardGem, 1);

        if(audioSource && successSound)
        {
            audioSource.PlayOneShot(successSound);
        }

        StartCoroutine(CooldownRoutine());
    }

    IEnumerator ActiveTimer()
    {
        yield return new WaitForSeconds(activeTime);

        if (raised)
        {
            StartCoroutine(CooldownRoutine());
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

        if(audioSource && sinkSound)
        {
            audioSource.PlayOneShot(sinkSound);
        }

        transform.position = buriedPosition;

        yield return new WaitForSeconds(respawnTime);

        onCooldown = false;

        SetGemVisible(true);
    }
}