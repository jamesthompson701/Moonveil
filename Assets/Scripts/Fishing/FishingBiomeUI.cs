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

    [Header("Net Visuals")]
    public SpriteRenderer fireNet;
    public SpriteRenderer waterNet;
    public SpriteRenderer earthNet;
    public SpriteRenderer airNet;

   [Header("Net Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(.4f, .4f, .4f, .25f);

    [Header("Bubble Camera")]
    public Transform bubbleCameraTarget;

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
    public ParticleSystem[] captureFX;
    public ParticleSystem[] bubbleFX;
    public ParticleSystem[] successFX;
    public ParticleSystem[] failFX;
}