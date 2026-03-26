using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public CanvasManager manager;
  public void CloseMenu()
    {
        manager.CloseTitleScreen();
    }
}
