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
    private Camera playerCamera;

    [Header("UI")]
    public TMP_Text startFishingPrompt;

    [Header("Element UI")]
    public Image requiredElementImage;
    public Sprite blankSprite;
    public Sprite fireSprite;
    public Sprite earthSprite;
    public Sprite waterSprite;
    public Sprite airSprite;

    [Header("Progress")]
    public Image catchProgressBar;

    [Header("Fishing Areas")]
    public FishingArea currentArea;

    [Header("Fishing Mode")]
    public bool inFishingMode;

    private ThirdPersonController playerController;
    private StarterAssetsInputs playerInput;
    private SpellManager2 spellManager;

    private SkinnedMeshRenderer[] playerMeshes;

    private List<FishingFish> currentCapturedFish = new List<FishingFish>();
    private Coroutine fishingPromptCoroutine;

    public GameObject nextQuest1;
    private bool nextQuest1Activated = false;


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
            
            //CanvasManager.Instance.CloseMiniGame(activeBiomeUI.fishingCanvas.gameObject);
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

        // Start in capture phase
        if (activeBiomeUI.captureCircle != null)
        {
            activeBiomeUI.captureCircle.SetActive(true);
        }

        if (activeBiomeUI.bubbleObject != null)
        {
            activeBiomeUI.bubbleObject.SetActive(false);
        }

        activeBiomeUI.fishingCamera.transform.position = activeBiomeUI.cameraAnchor.position;
        activeBiomeUI.fishingCamera.transform.rotation = activeBiomeUI.cameraAnchor.rotation;
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

        activeBiomeUI.captureCircle.SetActive(true);
        activeBiomeUI.bubbleObject.SetActive(false);

        //Debug.Log("Fishing Started");
        ClearRequiredElementUI();

        if (catchProgressBar != null)
        {
            catchProgressBar.fillAmount = 0f;
            catchProgressBar.gameObject.SetActive(false);
        }
    }

    public void StartCapturePhase()
    {
        currentPhase = FishingPhase.Capture;

        activeBiomeUI.captureCircle.SetActive(true);
        activeBiomeUI.bubbleObject.SetActive(false);

        currentCapturedFish.Clear();
        ClearRequiredElementUI();
    }

    public void StartBubblePhase(List<FishingFish> capturedFish)
    {
        currentCapturedFish.Clear();
        currentCapturedFish.AddRange(capturedFish);

        currentPhase = FishingPhase.Bubble;

        activeBiomeUI.captureCircle.SetActive(false);

        activeBiomeUI.bubbleObject.SetActive(true);
        activeBiomeUI.elementZones.SetActive(true);

        if (catchProgressBar != null)
        {
            catchProgressBar.fillAmount = 0f;
            catchProgressBar.gameObject.SetActive(true);
        }

        FishingBubble bubble = activeBiomeUI.bubbleObject.GetComponent<FishingBubble>();

        if (bubble != null)
        {
            if (currentCapturedFish.Count >= 5)
            {
                bubble.moveSpeed = 5f;
            }
            else
            {
                bubble.moveSpeed = 3f;
            }

            bubble.BeginBubblePhase();
        }
        //Debug.Log("Bubble phase started");
    }

    public void ExitFishingMode()
    {
        //Debug.Log("Exited Fishing");

        currentPhase = FishingPhase.None;

        activeBiomeUI.captureCircle.SetActive(false);
        activeBiomeUI.bubbleObject.SetActive(false);
        
        inFishingMode = false;

        if(activeBiomeUI.elementZones != null)
        {
            activeBiomeUI.elementZones.SetActive(false);
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
        ClearRequiredElementUI();

        if (catchProgressBar != null)
        {
            catchProgressBar.fillAmount = 0f;
            catchProgressBar.gameObject.SetActive(false);
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

        if (!nextQuest1Activated)
        {
            nextQuest1.SetActive(true);
            nextQuest1Activated = true;
        }

        ClearRequiredElementUI();
        currentCapturedFish.Clear();
        ExitFishingMode();
    }

    public void FailFishing()
    {
        //Debug.Log("Fishing Failed");

        /*foreach(FishingFish fish in currentCapturedFish)
        {
            fish.ResetFish();
        }*/
        ClearRequiredElementUI();
        currentCapturedFish.Clear();
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

    private void ClearRequiredElementUI()
    {
        if (requiredElementImage != null)
        {
            requiredElementImage.sprite = blankSprite;
        }
    }

    public void ShowFishingPrompt(string message)
    {
        if (startFishingPrompt == null) return;

        startFishingPrompt.text = message;
        startFishingPrompt.gameObject.SetActive(true);

        // Restart the timer if the prompt is shown again.
        if (fishingPromptCoroutine != null)
        {
            StopCoroutine(fishingPromptCoroutine);
        }

        fishingPromptCoroutine = StartCoroutine(HideFishingPromptAfterDelay());
    }

    private IEnumerator HideFishingPromptAfterDelay()
    {
        yield return new WaitForSeconds(8f);

        if (startFishingPrompt != null)
        {
            startFishingPrompt.gameObject.SetActive(false);
        }

        fishingPromptCoroutine = null;
    }

    public void UpdateCatchProgress(float progress)
    {
        if (catchProgressBar != null)
        {
            catchProgressBar.fillAmount = progress;
        }
    }
}