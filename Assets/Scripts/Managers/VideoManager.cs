using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public enum eVideos { cutscene}

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [NamedArray(typeof(eVideos))] public VideoClip[] videoClips;

    public static VideoManager Instance;

    public GameObject screenCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New VideoManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    public void PlayVideo(eVideos _video)
    {
        screenCanvas.SetActive(true);
        videoPlayer.clip = videoClips[(int)_video];
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(1);
    }
}
