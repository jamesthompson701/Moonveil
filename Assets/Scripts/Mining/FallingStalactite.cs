using System.Collections;
using UnityEngine;

public class FallingStalactite : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float fallDistance = 6f;
    [SerializeField] float fallDuration = 0.5f;
    [SerializeField] float respawnTime = 600f;

    [Header("Detection")]
    public Collider triggerCollider;

    private Vector3 ceilingPosition;
    private Vector3 groundPosition;

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

    void Start()
    {
        ceilingPosition = transform.position;
        groundPosition = ceilingPosition - transform.up * fallDistance;

        audioSource = GetComponent<AudioSource>();

        triggerCollider.enabled = true;

        foreach (Collider col in damageColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        falling = true;

        triggerCollider.enabled = false;

        foreach (Collider col in damageColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        PlayFX(fallFX);

        if (audioSource && fallSound)
        {
            audioSource.PlayOneShot(fallSound);
        }

        Vector3 startPos = transform.position;
        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, groundPosition, timer / fallDuration);
            yield return null;
        }

        transform.position = groundPosition;

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

        PlayFX(impactFX);

        falling = false;
        onCooldown = true;

        yield return new WaitForSeconds(respawnTime);

        transform.position = ceilingPosition;

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
}