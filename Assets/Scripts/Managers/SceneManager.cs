using UnityEngine;

/// <summary>
/// Handles scene transition logic, including loading and unloading scenes, managing scene states, and ensuring smooth transitions between different game levels or menus.
/// </summary>

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;

    public static SceneManager Instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadSceneTest()
    {         
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Demo Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Demo Scene");
    }

}
