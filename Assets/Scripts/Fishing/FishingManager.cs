using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StarterAssets;
using System.Collections.Generic;
using System.Collections;

public class FishingManager : MonoBehaviour
{

    public enum FishingPhase
    {
        None, Capture, Bubble
    }

    public FishingPhase currentPhase;

    public static FishingManager Instance;
    public FishingBiomeUI[] biomeUIs;
    private FishingBiomeUI activeBiomeUI;
    

    [Header("Player")]
    public GameObject player;
    public Transform cameraAnchor;
    private Camera playerCamera;

    [Header("UI")]
    public TMP_Text startFishingPrompt;

    [Header("Element UI")]
    public Image requiredElementImage;
    public Sprite fireSprite;
    public Sprite earthSprite;
    public Sprite waterSprite;
    public Sprite airSprite;

    [Header("Fishing Areas")]
    public FishingArea currentArea;

    [Header("Fishing Mode")]
    public bool inFishingMode;

    private ThirdPersonController playerController;
    private StarterAssetsInputs playerInput;
    private SpellManager2 spellManager;

    private SkinnedMeshRenderer[] playerMeshes;

    [Header("Capture Phase")]
    public GameObject captureCircle;


    [Header("Bubble Phase")]
    public GameObject bubbleObject;
    public GameObject elementZones;

    private List<FishingFish> currentCapturedFish = new List<FishingFish>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerController = FindFirstObjectByType<ThirdPersonController>();
        playerInput = FindFirstObjectByType<StarterAssetsInputs>();
        spellManager = FindFirstObjectByType<SpellManager2>();

        playerMeshes = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        playerCamera = Camera.main;
    }

    void Update()
    {
        // exit fishing
        if (inFishingMode && Input.GetKeyDown(KeyCode.Escape))
        {
            FailFishing();
            
            CanvasManager.Instance.CloseMiniGame(activeBiomeUI.fishingCanvas.gameObject);
        }
    }

    public void EnterFishingMode(FishingArea area)
    {
        //Debug.Log("Entered Fishing");

        if (area == null)
        {
            return;
        }

        inFishingMode = true;

        currentArea = area;

        // Start in capture phase
        if (captureCircle != null)
        {
            captureCircle.SetActive(true);
        }

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }

        activeBiomeUI = null;

        foreach (FishingBiomeUI ui in biomeUIs)
        {
            if (ui.biome == area.biome)
            {
                activeBiomeUI = ui;
                break;
            }
        }

        if (activeBiomeUI == null)
        {
            Debug.LogError("No FishingBiomeUI found for biome: " + area.biome);
            return;
        }

        activeBiomeUI.fishingCamera.transform.position = cameraAnchor.position;
        activeBiomeUI.fishingCamera.transform.rotation = cameraAnchor.rotation;
        activeBiomeUI.fishingCamera.gameObject.SetActive(true);
        activeBiomeUI.fishingCanvas.gameObject.SetActive(true);

        if(playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }

        // disable player
        if (playerInput)
        {
            playerInput.enabled = false;
        }

        if (playerController)
        {
            playerController.enabled = false;
        }

        ClickSelector selector = player.GetComponent<ClickSelector>();

        if (selector)
        {
            selector.enabled = false;
        }

        // hide player mesh
        foreach (var mesh in playerMeshes)
        {
            mesh.enabled = false;
        }

        // enable fishing visuals
        if (area.fishContainer != null)
        {
            area.fishContainer.gameObject.SetActive(true);
        }

        // cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentPhase = FishingPhase.Capture;

        captureCircle.SetActive(true);
        bubbleObject.SetActive(false);

        //Debug.Log("Fishing Started");
    }

    public void StartCapturePhase()
    {
        currentPhase = FishingPhase.Capture;

        captureCircle.SetActive(true);
        bubbleObject.SetActive(false);

        currentCapturedFish.Clear();
    }

    public void StartBubblePhase(List<FishingFish> capturedFish)
    {
        currentCapturedFish = capturedFish;

        currentPhase = FishingPhase.Bubble;

        captureCircle.SetActive(false);

        bubbleObject.SetActive(true);
        elementZones.SetActive(true);

        FishingBubble bubble = bubbleObject.GetComponent<FishingBubble>();

        bubble.BeginBubblePhase();

        //Debug.Log("Bubble phase started");
    }

    public void ExitFishingMode()
    {
        //Debug.Log("Exited Fishing");

        currentPhase = FishingPhase.None;

        captureCircle.SetActive(false);
        bubbleObject.SetActive(false);
        
        inFishingMode = false;

        if(elementZones != null)
        {
            elementZones.SetActive(false);
        }

        // enable player stuff
        if (playerInput)
        {
            playerInput.enabled = true;
        }

        if (playerController)
        {
            playerController.enabled = true;
        }

        if (activeBiomeUI != null)
        {
            activeBiomeUI.fishingCanvas.gameObject.SetActive(false);
        }

        ClickSelector selector = player.GetComponent<ClickSelector>();

        if (selector)
        {
            selector.enabled = true;
        }

        // show player mesh
        foreach (var mesh in playerMeshes)
        {
            mesh.enabled = true;
        }

        // disable fishing visuals
        if (currentArea != null && currentArea.fishContainer != null)
        {
            currentArea.fishContainer.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentArea = null;

        activeBiomeUI.fishingCamera.gameObject.SetActive(false);
        activeBiomeUI.fishingCanvas.gameObject.SetActive(false);

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        if (activeBiomeUI != null)
        {
            activeBiomeUI.fishingCanvas.gameObject.SetActive(false);
        }

        //Debug.Log("Fishing Ended");
    }

    public void SuccessFishing()
    {
        //Debug.Log("Fishing Success");

        foreach(FishingFish fish in currentCapturedFish)
        {
            fish.RemoveFish(180f);

            if(fish.fishData != null)
            {
                InventoryManager.instance.AddFish(fish.fishData, 1);
            }
        }
        ExitFishingMode();
    }

    public void FailFishing()
    {
        //Debug.Log("Fishing Failed");

        foreach(FishingFish fish in currentCapturedFish)
        {
            fish.ResetFish();
        }

        ExitFishingMode();
    }

    public void SetCurrentArea(FishingArea area)
    {
        currentArea = area;
    }

    public void ClearCurrentArea(FishingArea area)
    {
        if (currentArea == area)
        {
            currentArea = null;
        }
    }

    public void SetRequiredElementUI(ElementType element)
{
    if(requiredElementImage == null)
    {
        return;
    }

    switch(element)
    {
        case ElementType.Fire:
            requiredElementImage.sprite = fireSprite;
            break;

        case ElementType.Earth:
            requiredElementImage.sprite = earthSprite;
            break;

        case ElementType.Water:
            requiredElementImage.sprite = waterSprite;
            break;

        case ElementType.Air:
            requiredElementImage.sprite = airSprite;
            break;
    }
}
}