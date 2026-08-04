using UnityEngine;
using TMPro;
using UnityEngine.UI;


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
    public Image requiredElementImage;
    public Slider catchProgressBar;

    [Header("Capture")]
    public GameObject captureCircle;

    [Header("Bubble")]
    public GameObject bubbleObject;
    public GameObject elementZones;

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip captureSound;
    public AudioClip bubbleSound;
    public AudioClip successSound;
    public AudioClip failSound;


    [Header("VFX")]
    public ParticleSystem captureFX;
    public ParticleSystem bubbleFX;
    public ParticleSystem successFX;
    public ParticleSystem failFX;
}