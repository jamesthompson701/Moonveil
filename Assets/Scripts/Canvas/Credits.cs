using UnityEngine;

public class Credits : MonoBehaviour
{

    public void OnBackClicked()
    {
        Instantiate(Resources.Load("Canvas/" + "TitleScreen") as GameObject);
        DestroyCanvas();
    }

    void DestroyCanvas()
    {
        Destroy(this.gameObject);
    }


}
