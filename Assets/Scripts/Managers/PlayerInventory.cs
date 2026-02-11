using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class PlayerInventory : MonoBehaviour
{
    //this might be a temporary script
    //just tracks seeds
    public static PlayerInventory instance;
    public InventorySO invSO;

    public static int seeds;
    public static int fish;

    public ItemSO fishRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ResetSeeds();
        ResetInv();
    }

    //EVERYTHING IS SEEDS

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
        if (invSO.InventoryItems[0].amount != 5)
        {
            invSO.InventoryItems[0].amount = 5;
        }
        seeds = invSO.InventoryItems[0].amount;
    }

    //EVERYTHING IS FISH

    public void AddFish(int _amount)
    {
        if (invSO.InventoryItems.Any(item => item.item.itemName == "IceFish"))
        {
            fish = invSO.InventoryItems[1].amount + _amount;
            invSO.InventoryItems[1].amount = fish;
        }
        else
        {
            invSO.AddItem(fishRef, _amount);
        }
        

    }

    public void ResetInv()
    {
        if (invSO.InventoryItems[1].amount != 0)
        {
            invSO.InventoryItems.RemoveAt(1);
        }

        fish = 0;
    }

}
