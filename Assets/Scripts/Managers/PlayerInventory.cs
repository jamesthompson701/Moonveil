using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class PlayerInventory : MonoBehaviour
{
    //just tracks seeds
    // ^does not, in fact, just track seeds anymore
    public static PlayerInventory instance;
    public InventorySO invSO;

    public SoilObject soilRef;

    public int newtSeeds;
    public int woolSeeds;
    public int lizardSeeds;
    public static int fish;

    //player currency counter
    public int crescants;

    public SeedItemSO seedRef;
    public ItemSO fishRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ResetInv();
    }

    //EVERYTHING IS SEEDS - no longer; we are now seed agnostic
    public int CheckSeeds()
    {
        if (seedRef.itemName == "Eye Of Newt Seed")
        {
            return newtSeeds;
        }
        else if (seedRef.itemName == "Wool Of Bat Seed")
        {
            return woolSeeds;
        }
        else
        {
            return lizardSeeds;
        }
    }

    //EVERYTHING IS FISH

    public void AddFish(int _amount)
    {
        if (invSO.InventoryItems.Any(item => item.item.itemName == "IceFish"))
        {
            fish = invSO.InventoryItems[3].amount + _amount;
            invSO.InventoryItems[3].amount = fish;
        }
        else
        {
            invSO.AddItem(fishRef, _amount);
        }
        

    }
    public void ResetInv()
    {
        invSO.InventoryItems.Clear();
        fish = 0;
    }

}
