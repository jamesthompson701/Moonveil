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
            EnterFishingMode(currentArea);
        }

        // Exit fishing (likely to change input)
        if (inFishingMode && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitFishingMode();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1 working");
        }
        
    }

    public void EnterFishingMode(FishingArea area)
    {
        if (area == null)
        {
            Debug.LogError("FishingArea is NULL");
            return;
        }

        // disable movement input
        if (playerInput != null)
            playerInput.enabled = false;

        // disable spell casting
        if (spellManager != null)
        {
            spellManager.attackChoice = 0;
            spellManager.enabled = false;
        }

        if (startFishingPrompt != null)
        startFishingPrompt.gameObject.SetActive(false);

        inFishingMode = true;
        activeBiomeUI = null;
        

        playerController.enabled = false;
        player.GetComponent<ClickSelector>().enabled = false;

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

       // enable this biome's systems
        activeBiomeUI.fishingCamera.gameObject.SetActive(true);
        activeBiomeUI.fishingCanvas.gameObject.SetActive(true);

        if (activeBiomeUI.fishingVisuals != null)
        {
            activeBiomeUI.fishingVisuals.SetActive(true);

            foreach (Transform child in activeBiomeUI.fishingVisuals.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        //rod stuff
        currentRod = activeBiomeUI.rod;
        if (currentRod != null)
        {
            currentRod.Initialize(this, reelInput);
            currentRod.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No rod assigned for biome: " + activeBiomeUI.biome);
        }

        Transform bubble = activeBiomeUI.fishingVisuals.transform.Find("FishBubble");
        if (bubble != null)
        bubble.gameObject.SetActive(false);

        inFishingMode = true;
        currentArea = area;

        // switch cameras
        if (mainCamera) mainCamera.enabled = false;

        fishingCamera = activeBiomeUI.fishingCamera;
        fishingCamera.gameObject.SetActive(true);
        fishingCamera.enabled = true;

        activeBiomeUI.fishingCanvas.gameObject.SetActive(true);
        miniGameUI = activeBiomeUI.miniGameUI;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ShowPrompt("Press " + castInput + " to cast your rod");
        Debug.Log("Entered fishing mode in area: " + area.areaName + ". Press " + startFishingInput + " to cast (or " + castInput + " depending on config).");
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
        Debug.Log("Rod pulled - canceled casting");
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
            ShowPrompt("Press " + reelInput + " to reel!");
            Debug.Log("Fish is biting! Press '" + reelInput + "' to reel fish.");

            // window to start reeling
            float reelWindow = 2.5f; // configurable globally or per fish if you want
            float timer = 0f;
            bool startedMinigame = false;
            while (timer < reelWindow)
            {
                if (Input.GetButtonDown(reelInput))
                {
                    // begin minigame
                    Debug.Log("Starting reeling minigame...");
                    startedMinigame = true;
                    StartMiniGame(fish);
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (!startedMinigame)
            {
                Debug.Log("Player didn't reel in time. Fish got away (or next opportunity starts).");
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
        ShowPrompt("Keep the blue ring inside the dark rings! " + "Press " + reelInput + " to shrink the ring!");

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
            Debug.Log("You caught a " + caughtFish.fishName + "!");

            // add fish to inventory.
            // notify inventory / UI systems
            OnFishCaught?.Invoke(caughtFish);


            InventoryManager.instance.AddFish(caughtFish, 1);

            //tutorial
            if (!TutorialManager.instance.fishingDone)
            {
                TutorialManager.instance.ProgressTutorial(6);
                TutorialManager.instance.fishingDone = true;
            }

        }
        else
        {
            Debug.Log("Fish escaped.");
            // keep the line casted? For this version, keep casted so next opportunity continues
        }

        if (success)
        {
            ShowPrompt(caughtFish.fishName + " fish caught!" + "  Press " + castInput + " to recast your rod" + " or press Escape to exit");
        }
        else
        {
            ShowPrompt("The fish escaped..." + " Press " + castInput + " to recast your rod" + " or press Escape to exit");
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