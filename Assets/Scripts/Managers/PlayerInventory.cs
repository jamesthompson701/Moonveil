using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlayerInventory : MonoBehaviour
{
    //this might be a temporary script
    //just tracks seeds
    public static PlayerInventory instance;
    public InventorySO invSO;

    public static int seeds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ResetSeeds();
    }

    public void AddSeeds(int _amount)
    {
        seeds = invSO.InventoryItems[0].amount + _amount;
        invSO.InventoryItems[0].amount = seeds;
    }

    public int CheckSeeds()
    {
        return seeds;
    }

    public void ResetSeeds()
    {
        if (invSO.InventoryItems[0].amount <5)
        {
            invSO.InventoryItems[0].amount = 5;
        }
        seeds = invSO.InventoryItems[0].amount;
    }

}
