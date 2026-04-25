using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public Camera miningCamera;
    public Camera mainCamera;

    public Transform miningSpawnPoint;
    public Transform returnPoint;

    public MiningMiniGameUI miniGameUI;

    private MineRock currentRock;

    public bool isMining = false;

    public ItemSO temporaryOutputGem;

    public static MiningManager Instance;

    public void Awake()
    {
        //Making Mining manager a singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New Spell Manager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartMining(MineRock rock)
    {
        if (isMining) return;

        Debug.Log("StartMining called");

        isMining = true;
        currentRock = rock;

        StartCoroutine(StartMiningRoutine());
    }

    IEnumerator StartMiningRoutine()
    {
        Debug.Log("Mining routine started");

        yield return Fade(1f);

        Debug.Log("Fade complete");

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        //ClickSelector.Instance.enabled = false;
        Debug.Log("miniGameUI: " + miniGameUI);
        Debug.Log("currentRock: " + currentRock);
        Debug.Log("cameraAnchor: " + currentRock.cameraAnchor);
        Debug.Log("miningCamera: " + miningCamera);

        Debug.Log("CanvasManager Instance: " + CanvasManager.Instance);
        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.OpenMiniGame(miniGameUI.gameObject);
        }
        else
        {
            Debug.LogError("CanvasManager Instance is NULL");
        }

        mainCamera.gameObject.SetActive(false);
        miningCamera.gameObject.SetActive(true);

        miningCamera.transform.position = currentRock.cameraAnchor.position;
        miningCamera.transform.rotation = currentRock.cameraAnchor.rotation;

        miniGameUI.StartMiniGame(currentRock.state);

        if (HUD.instance != null)
        HUD.instance.gameObject.SetActive(false);

        yield return Fade(0f);
    }

    public void EndMining(bool success)
    {
        if (!isMining) return;

        if (success)
        {
            //tutorial
            if (TutorialManager.instance != null && !TutorialManager.instance.mining)
            {
                //completes billboard 8; mine a gem
                TutorialManager.instance.ProgressTutorial(8);
                TutorialManager.instance.mining = true;
            }

            GiveReward(currentRock);
        }
        else
        {
            currentRock.Fail();
        }

        StartCoroutine(EndMiningRoutine());
    }

    IEnumerator EndMiningRoutine()
    {
        yield return Fade(1f);

        // Switch cameras FIRST
        miningCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        // Restore HUD
        if (HUD.instance != null)
            HUD.instance.gameObject.SetActive(true);

        // Close UI system LAST
        if (CanvasManager.Instance != null)
            CanvasManager.Instance.CloseMiniGame(miniGameUI.gameObject);

        isMining = false;

        yield return Fade(0f);
    }

    void GiveReward(MineRock rock)
    {
        int amount = 1;

        if (rock.state == RockState.Fresh)
            amount = Random.Range(3, 6);
        else if (rock.state == RockState.Cracked)
            amount = Random.Range(1, 3);

        Debug.Log("Gained " + amount + " " + rock.mineralType + " gems");

        // just give the player a generic gem for now
        InventoryManager.instance.invSO.AddItem(temporaryOutputGem, 1);
        //InventoryManager.instance.invSO.AddItem(rock.mineralType, 2);
    }

    // screen fade
    public CanvasGroup fadeCanvas;
    public float fadeSpeed = 2f;

    IEnumerator Fade(float target)
    {
        while (!Mathf.Approximately(fadeCanvas.alpha, target))
        {
            fadeCanvas.alpha = Mathf.MoveTowards(
                fadeCanvas.alpha,
                target,
                fadeSpeed * Time.deltaTime);

            yield return null;
        }
    }
}