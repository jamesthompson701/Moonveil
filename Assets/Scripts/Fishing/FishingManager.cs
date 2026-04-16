using System;
using System.Collections;
using UnityEngine;
using TMPro;
using StarterAssets;

public class FishingManager : MonoBehaviour
{
  ThirdPersonController playerController;
    StarterAssetsInputs playerInput;
    SpellManager spellManager;
    public static event Action<FishData> OnFishCaught;

    [Header("References")]
    public Camera mainCamera;
    public Camera fishingCamera; // current area's camera; swapped to
    public GameObject player;
    public FishingRod fishingRodPrefab;
    public Transform rodParent; // where to parent the rod when shown
    public FishingMiniGameUI miniGameUI;

    [Header("UI Prompt")]
    [Header("Biome UI")]
    public FishingBiomeUI[] biomeUIs;
    private FishingBiomeUI activeBiomeUI;

    [System.Serializable]
    public class FishingBiomeUI
    {
        public FishingBiome biome;
        public Camera fishingCamera;
        public Canvas fishingCanvas;
        public FishingMiniGameUI miniGameUI;
        public TMP_Text promptText;

         public GameObject fishingVisuals;
         public FishingRod rod;
    }

    [Header("Gameplay Prompt")]
    public TMP_Text startFishingPrompt;

    [Header("Input Settings")]
    public string startFishingInput = "Fire2"; // right mouse to start
    public string castInput = "Fire1";         // left mouse to cast
    public string reelInput = "Jump";          // space to start reeling when bite occurs, and used inside minigame

    [Header("Timings")]
    public float postCastDelay = 1f; // delay between cast and bait appearing

    public FishingArea currentArea;

    public FishingRod currentRod;
    private Coroutine biteCoroutine;
    private bool inFishingMode = false;
    private bool lineIsCasted = false;

    public static FishingManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        if (startFishingPrompt == null)
            Debug.LogError("StartFishingPrompt is NOT assigned!");

        if (biomeUIs.Length == 0)
            Debug.LogError("No biome UIs configured!");
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (miniGameUI != null)
            miniGameUI.gameObject.SetActive(false);

        playerController = FindFirstObjectByType<ThirdPersonController>();
        playerInput = FindFirstObjectByType<StarterAssetsInputs>();
        spellManager = FindFirstObjectByType<SpellManager>();
    }

    void Update()
    {
        // Start fishing when in a fishing area and player presses startFishingInput
        if (currentArea != null && !inFishingMode && Input.GetButtonDown(startFishingInput))
        {
            // im just adding this so that the book UI doesnt pop up when u esc
            CanvasManager.Instance.OpenMiniGame();

            EnterFishingMode(currentArea);
        }

        // Exit fishing (likely to change input)
        if (inFishingMode && Input.GetKeyDown(KeyCode.Escape))
        {
            if (miniGameUI != null && miniGameUI.IsActiveAndPlaying) return;
            ExitFishingMode();
            
            //same reason as previous comment
            CanvasManager.Instance.CloseMiniGame();
        }

        // Show "Start Fishing" prompt
        if (currentArea != null && !inFishingMode)
        {
            startFishingPrompt.gameObject.SetActive(true);
            startFishingPrompt.text = "Press " + startFishingInput + " to start fishing";
        }
        else if (startFishingPrompt != null)
        {
            startFishingPrompt.gameObject.SetActive(false);
        }
    }

    public void EnterFishingMode(FishingArea area)
{
    if (area == null)
    {
        Debug.LogError("FishingArea is NULL");
        return;
    }

    // Disable player systems
    if (playerInput) playerInput.enabled = false;

    if (spellManager)
    {
        spellManager.attackChoice = 0;
        spellManager.enabled = false;
    }

    if (startFishingPrompt)
        startFishingPrompt.gameObject.SetActive(false);

    playerController.enabled = false;
    player.GetComponent<ClickSelector>().enabled = false;

    // Find correct biome UI
    activeBiomeUI = null;

    foreach (var ui in biomeUIs)
    {
        ui.fishingCamera.gameObject.SetActive(false);
        ui.fishingCanvas.gameObject.SetActive(false);

        if (ui.biome == area.biome)
        {
            activeBiomeUI = ui;
        }
    }

    if (activeBiomeUI == null)
    {
        Debug.LogError("No UI configured for biome: " + area.biome);
        return;
    }

    // Enable visuals safely
    if (activeBiomeUI.fishingVisuals != null)
    {
        activeBiomeUI.fishingVisuals.SetActive(true);

        foreach (Transform child in activeBiomeUI.fishingVisuals.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Setup rod (USE MAIN BRANCH STYLE)
    currentRod = activeBiomeUI.rod;

    if (currentRod != null)
    {
        currentRod.Initialize(this, reelInput);
        currentRod.gameObject.SetActive(true);
    }
    else
    {
        Debug.LogError("Rod missing for biome: " + activeBiomeUI.biome);
    }

    // Cameras
    if (mainCamera) mainCamera.enabled = false;

    fishingCamera = activeBiomeUI.fishingCamera;
    fishingCamera.gameObject.SetActive(true);
    fishingCamera.enabled = true;

    activeBiomeUI.fishingCanvas.gameObject.SetActive(true);
    miniGameUI = activeBiomeUI.miniGameUI;

    currentArea = area;
    inFishingMode = true;

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    ShowPrompt("Cast your rod!");
}

    public void ExitFishingMode()
    {
        if (playerInput != null)
        playerInput.enabled = true;

        if (spellManager != null)
        spellManager.enabled = true;

        inFishingMode = false;
        currentArea = null;

        player.GetComponent<ThirdPersonController>().enabled = true;
        player.GetComponent<ClickSelector>().enabled = true;

        if (activeBiomeUI != null && activeBiomeUI.fishingVisuals != null)
        activeBiomeUI.fishingVisuals.SetActive(false);

        if (mainCamera) mainCamera.enabled = true;
        if (fishingCamera) fishingCamera.enabled = false;

        if (currentRod) currentRod.gameObject.SetActive(false);

        if (biteCoroutine != null)
        {
            StopCoroutine(biteCoroutine);
            biteCoroutine = null;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        lineIsCasted = false;
        HidePrompt();
        if (miniGameUI) miniGameUI.EndGameCleanup();
    }

    // Called by FishingRod when cast completed
    public void OnRodCasted(Vector3 baitPosition)
    {
        if (lineIsCasted) return;
        lineIsCasted = true;
        // start bite loop
        biteCoroutine = StartCoroutine(BiteLoop());
        ShowPrompt("Waiting for a bite...");
    }

    // Called by FishingRod when rod is pulled (cancel cast)
    public void OnRodPulled()
    {
        lineIsCasted = false;
        if (biteCoroutine != null)
        {
            StopCoroutine(biteCoroutine);
            biteCoroutine = null;
        }
    }

    IEnumerator BiteLoop()
    {
        while (lineIsCasted)
        {
            // pick a fish for this opportunity
            FishData fish = currentArea.GetRandomFish();
            if (fish == null)
            {
                Debug.Log("No fish configured for this area.");
                yield break;
            }

            // random bite time per fish
            float wait = UnityEngine.Random.Range(fish.biteDelayMin, fish.biteDelayMax);
            yield return new WaitForSeconds(wait);

            // fish bites now -> prompt player to reel
            ShowPrompt("Reel!");

            // window to start reeling
            float reelWindow = 2.5f; // configurable globally or per fish if you want
            float timer = 0f;
            bool startedMinigame = false;
            while (timer < reelWindow)
            {
                if (Input.GetButtonDown(reelInput))
                {
                    // begin minigame
                    startedMinigame = true;
                    StartMiniGame(fish);
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (!startedMinigame)
            {
                // continue loop to wait for next bite (cast remains)
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // the minigame drives the result; wait until it finishes before continuing the outer loop
                while (miniGameUI.IsActiveAndPlaying)
                    yield return null;

                // after game ends: if rod is no longer casted (on success you might auto-pull), handle accordingly
                // For now, we reset lineIsCasted to false only if the fishing rod code pulled it.
                // Wait a tiny bit then allow next chance if still casted
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    void StartMiniGame(FishData fish)
    {
        Transform bubble = activeBiomeUI.fishingVisuals.transform.Find("FishBubble");
        if (bubble) bubble.gameObject.SetActive(true);

        if (miniGameUI == null)
        {
            Debug.Log("MiniGameUI not assigned in FishingManager.");
            return;
        }
        ShowPrompt("Keep the White Ring between the Dark Rings");

        // configure the mini game from fish parameters
        miniGameUI.gameObject.SetActive(true);
        miniGameUI.StartMiniGame(fish, reelInput, OnMiniGameResult);
    }

    // callback from mini game
    void OnMiniGameResult(bool success, FishData caughtFish)
    {
        HidePrompt();

        Transform bubble = activeBiomeUI.fishingVisuals.transform.Find("FishBubble");
        if (bubble) bubble.gameObject.SetActive(false);

        if (success)
        {
            // add fish to inventory.
            // notify inventory / UI systems
            OnFishCaught?.Invoke(caughtFish);


            InventoryManager.instance.AddFish(caughtFish, 1);


        }
        else
        {
            // keep the line casted? For this version, keep casted so next opportunity continues
        }

        if (success)
        {
            //tutorial
            if (TutorialManager.instance != null && !TutorialManager.instance.fishing)
            {
                //completes billboard 6: go fishing
                TutorialManager.instance.ProgressTutorial(6);
                TutorialManager.instance.fishing = true;
            }

            ShowPrompt(caughtFish.fishName + " caught!" + " Recast your rod or exit");
        }
        else
        {
            ShowPrompt("The fish escaped..." + " Recast your rod or exit");
        }
    }

    public void ShowPrompt(string message)
    {
        if (activeBiomeUI == null || activeBiomeUI.promptText == null) return;

        activeBiomeUI.promptText.text = message;
        activeBiomeUI.promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (activeBiomeUI == null || activeBiomeUI.promptText == null) return;

        activeBiomeUI.promptText.gameObject.SetActive(false);
    }

    public void SetCurrentArea(FishingArea area)
    {
        currentArea = area;

        if (!inFishingMode && startFishingPrompt != null)
        {
            startFishingPrompt.text = "Press " + startFishingInput + " to start fishing";
            startFishingPrompt.gameObject.SetActive(true);
        }
    }

    public void ClearCurrentArea(FishingArea area)
    {
        if (currentArea == area)
        {
            currentArea = null;

            if (startFishingPrompt != null)
                startFishingPrompt.gameObject.SetActive(false);
        }
    }
}