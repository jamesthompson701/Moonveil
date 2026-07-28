using UnityEngine;
using TMPro;

[System.Serializable]
public class FishingBiomeUI
{
    [Header("Biome")]
    public FishingBiome biome;

    [Header("Camera")]
    public Camera fishingCamera;
    public Transform cameraAnchor;

    [Header("UI")]
    public Canvas fishingCanvas;

    [Header("Capture")]
    public GameObject captureCircle;

    [Header("Bubble")]
    public GameObject bubbleObject;
    public GameObject elementZones;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip raiseSound;
    public AudioClip successSound;
    public AudioClip failSound;

    [Header("VFX")]
    public ParticleSystem raiseFX;
    public ParticleSystem successFX;
    public ParticleSystem failFX;
}