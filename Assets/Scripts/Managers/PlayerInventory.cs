using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //this might be a temporary script
    //just tracks Supposed Cactus seeds
    public static PlayerInventory instance;

    public int seeds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddSeeds(int _amount)
    {
        seeds = seeds + _amount;
    }

}
