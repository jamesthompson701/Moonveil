using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //this might be a temporary script
    //just tracks Supposed Cactus seeds
    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


}
