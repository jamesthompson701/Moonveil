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

    //EVERYTHING IS SEEDS
    public int CheckSeeds()
    {
        foreach (var item in invSO.InventoryItems)
        {
            if (item.item.itemID == seedRef.itemID)
            {
                return item.amount;
            }
        }
        return 0;
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
