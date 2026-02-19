using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
