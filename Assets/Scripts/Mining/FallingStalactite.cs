using System.Collections;
using UnityEngine;

public class FallingStalactite : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float fallDistance = 17f;
    [SerializeField] float fallDuration = 0.5f;
    [SerializeField] float respawnTime = 600f;

    [Header("Detection")]
    public Collider triggerCollider;

   private Vector3 ceilingPosition;
    private Vector3 groundPosition;

    [SerializeField] private Transform stoneBody;

    private bool falling;
    private bool onCooldown;

    [Header("Damage")]
    public Collider[] damageColliders;

    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip fallSound;
    public AudioClip impactSound;

    [Header("VFX")]
    public ParticleSystem[] fallFX;
    public ParticleSystem[] impactFX;

    [Header("Floating")]
    [SerializeField] private float floatRotationSpeed = 20f;
    [SerializeField] private float floatRotationAmount = 8f;

    private Quaternion startRotation;

    void Start()
    {
        ceilingPosition = stoneBody.position;
        groundPosition = ceilingPosition + Vector3.down * fallDistance;

        audioSource = GetComponent<AudioSource>();

        triggerCollider.enabled = true;

        foreach (Collider col in damageColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        startRotation = stoneBody.localRotation;
    }

    void Update()
    {
        if (falling)
            return;

        float x = Mathf.Sin(Time.time * 0.8f) * 2f;
        float z = Mathf.Sin(Time.time * 1.1f) * 2f;

        stoneBody.localRotation = startRotation * Quaternion.Euler(x, 0f, z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Stalactite Trigger entered by " + other.name);

        if (onCooldown || falling)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            StartCoroutine(FallRoutine());
        }
    }

    IEnumerator FallRoutine()
    {
        Debug.Log("FallRoutine Started");

        falling = true;

        triggerCollider.enabled = false;

        foreach (Collider col in damageColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        StartCoroutine(PlayFXOnce(fallFX));

        if (audioSource && fallSound)
        {
            audioSource.PlayOneShot(fallSound);
        }

        Vector3 startPos = stoneBody.position;
        float timer = 0f;

        Debug.Log("Starting position: " + startPos);
        Debug.Log("Target position: " + groundPosition);

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            stoneBody.position = Vector3.Lerp(startPos,groundPosition,timer / fallDuration);

            Debug.Log("Moving: " + stoneBody.position);
            yield return null;
        }

        stoneBody.position = groundPosition;

        Debug.Log("Finished falling");

        foreach (Collider col in damageColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        if (audioSource && impactSound)
        {
            audioSource.PlayOneShot(impactSound);
        }

        StartCoroutine(PlayFXOnce(impactFX));

        falling = false;
        onCooldown = true;

        yield return new WaitForSeconds(respawnTime);

        stoneBody.position = ceilingPosition;

        yield return new WaitForSeconds(0.25f);

        onCooldown = false;

        triggerCollider.enabled = true;
    }

    void PlayFX(ParticleSystem[] effects)
    {
        if (effects == null) 
        {
            return;
        }

        foreach (ParticleSystem fx in effects)
        {
            if (fx == null) 
            {
                continue;
            }

            fx.gameObject.SetActive(true);
            fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fx.Clear();
            fx.Play();
        }
    }

    IEnumerator PlayFXOnce(ParticleSystem[] effects)
    {
        if (effects == null)
        {
            yield break;
        }

        foreach (ParticleSystem fx in effects)
        {
            if (fx == null)
            {
                continue;
            }

            fx.gameObject.SetActive(true);
            fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fx.Clear();
            fx.Play();
        }

        // Wait for the longest particle effect to finish
        yield return new WaitForSeconds(2f);

        foreach (ParticleSystem fx in effects)
        {
            if (fx != null)
            {
                fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                fx.gameObject.SetActive(false);
            }
        }
    }
}