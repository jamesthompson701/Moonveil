using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
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
}
