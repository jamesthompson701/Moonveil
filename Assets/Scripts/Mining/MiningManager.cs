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

    private bool isMining = false;


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
        CanvasManager.Instance.OpenMiniGame();

        mainCamera.gameObject.SetActive(false);
        miningCamera.gameObject.SetActive(true);

        miningCamera.transform.position = currentRock.cameraAnchor.position;
        miningCamera.transform.rotation = currentRock.cameraAnchor.rotation;

        miniGameUI.StartMiniGame(currentRock.state);

        yield return Fade(0f);
    }

    public void EndMining(bool success)
    {
        if (success)
        {
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

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //ClickSelector.Instance.enabled = true;
        CanvasManager.Instance.CloseMiniGame();

        miningCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

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

        // Later:
        // INVENTORY ADD (rock.mineralType, amount);

        //InventoryManager.instance.invSO.AddItem(rock.mineralType., 2);
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