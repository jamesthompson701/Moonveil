using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        //VideoManager.Instance.PlayVideo(eVideos.cutscene);
        //Destroy(gameObject);
    }
}
