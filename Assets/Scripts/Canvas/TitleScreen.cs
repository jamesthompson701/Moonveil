using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    [SerializeField] private GameObject optionsCanvas;
    [SerializeField] private GameObject creditsCanvas;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        //VideoManager.Instance.PlayVideo(eVideos.cutscene);
        //Destroy(gameObject);
    }

    public void OpenOptions()
    {
        Debug.Log("Options Button Clicked");

        Instantiate(Resources.Load("Canvas/" + "OptionsTitleScreen") as GameObject);
        DestroyCanvas();
        //optionsCanvas.SetActive(true);
        //optionsCanvas.transform.SetAsLastSibling();
    }

    public void OpenCredits()
    {
        Debug.Log("Credits Button Clicked");

        Instantiate(Resources.Load("Canvas/" + "Credits") as GameObject);
        DestroyCanvas();
    }

    public void CloseOptions()
    {
        optionsCanvas.SetActive(false);
    }

    void DestroyCanvas()
    {
        Destroy(this.gameObject);
    }
}
