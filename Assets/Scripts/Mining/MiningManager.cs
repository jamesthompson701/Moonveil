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

    public void StartMining(MineRock rock)
    {
        currentRock = rock;

        StartCoroutine(StartMiningRoutine());
    }

    IEnumerator StartMiningRoutine()
    {
        yield return Fade(1f);

        mainCamera.gameObject.SetActive(false);
        miningCamera.gameObject.SetActive(true);

        miningCamera.transform.position = currentRock.cameraAnchor.position;
        miningCamera.transform.rotation = currentRock.cameraAnchor.rotation;

        miniGameUI.StartMiniGame(currentRock.state);

        yield return Fade(0f);
    }

    public void EndMining(bool success)
    {
        if (!success)
        {
            currentRock.Fail();
        }

        StartCoroutine(EndMiningRoutine());
    }

    IEnumerator EndMiningRoutine()
    {
        yield return Fade(1f);

        miningCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        yield return Fade(0f);
    }

    // SIMPLE FADE
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