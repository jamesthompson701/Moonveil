using UnityEngine;

public class WorldTree : MonoBehaviour
{
    public GameObject myMenu;

    public virtual void OnInteract()
    {
        myMenu.SetActive(true);
    }

}
