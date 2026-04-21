using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.Windows;


public enum eVideos { cutscene}

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [NamedArray(typeof(eVideos))] public VideoClip[] videoClips;

    InputActionMap player;

    public static VideoManager Instance;

    public InputActionAsset input;

    public GameObject screenCanvas;

    InputAction skip;

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
        player = input.FindActionMap("Player");

        player.Enable();

        skip = input.FindAction("Pause");

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        if (skip.WasPressedThisFrame())
        {
            videoPlayer.time = videoPlayer.length;
        }
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
