using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenshotToBook : MonoBehaviour
{
    [SerializeField] private Image bookPageImage;

    private Texture2D pauseScreenshot;
    private Sprite pauseScreenshotSprite;

    public void PauseAndOpenBook()
    {
        StartCoroutine(CaptureThenOpenMenu());
    }

    private IEnumerator CaptureThenOpenMenu()
    {
       // Wait for gameplay frame to fully render
       yield return new WaitForEndOfFrame();

        //Destroy any old screenshots so we don't use up all our memory on                            //screenshots
        if (pauseScreenshot != null)
        {
            Destroy(pauseScreenshot);
        }

        if (pauseScreenshotSprite != null)
        {
            Destroy(pauseScreenshotSprite);
        }

        int width = Screen.width;
        int height = Screen.height;
        
        // Capture screen
        pauseScreenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        pauseScreenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        pauseScreenshot.Apply();

        // Convert to sprite
        pauseScreenshotSprite = Sprite.Create(
            pauseScreenshot,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f)
        );

        // Assign to UI image
        bookPageImage.sprite = pauseScreenshotSprite;

        // NOW open the menu
        CanvasManager.Instance.OpenMenu(1);
    }

    private void OnDestroy()
    {
        if (pauseScreenshot != null)
        {
            Destroy(pauseScreenshot);
        }

        if (pauseScreenshotSprite != null)
        {
            Destroy(pauseScreenshotSprite);
        }
    }
}