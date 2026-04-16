using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void PlayGame()
    {
        VideoManager.Instance.PlayVideo(eVideos.cutscene);
        Destroy(gameObject);
    }
}
