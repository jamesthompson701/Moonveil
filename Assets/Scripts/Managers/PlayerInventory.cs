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
    public DatabaseSO database;
    
    //tracks amount of each item
    public static int newtSeeds;
    public static int woolSeeds;
    public static int lizardSeeds;
    public static int fish;

    public SeedItemSO curSeed;
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

    //public void AddSeeds(int _amount, SeedItemSO _type)
    //{
    //    //three seed kinds
    //    switch (_type.itemName)
    //    {
    //        case "Eye Of Newt Seed":
    //            if (invSO.InventoryItems.Any(item => item.item.itemID == 8))
    //            {
    //                fish = invSO.InventoryItems[2].amount + _amount;
    //                invSO.InventoryItems[2].amount = fish;
    //            }

    //            newtSeeds = invSO.InventoryItems[0].amount + _amount;
          
    //            invSO.InventoryItems[0].amount = newtSeeds;
    //            break;
    //        case "Wool Of Bat Seed":
    //            woolSeeds = invSO.InventoryItems[1].amount + _amount;
    //            invSO.InventoryItems[1].amount = woolSeeds;
    //            break;
    //        case "Lizard's Legs Seed":
    //            lizardSeeds = invSO.InventoryItems[2].amount + _amount;
    //            invSO.InventoryItems[1].amount = lizardSeeds;
    //            break;
    //    }

    //}


    public int CheckSeeds()
    {
        foreach(var item in invSO.InventoryItems)
        {
            if (item.item.itemID == curSeed.itemID)
            {
                return item.amount;
            }
        }
        return 0;
        //if (curSeed.itemName == "Eye Of Newt Seed")
        //{
        //    return newtSeeds;
        //}
        //else if (curSeed.itemName == "Wool Of Bat Seed")
        //{
        //    return woolSeeds;
        //}
        //else
        //{
        //    return lizardSeeds;
        //}
    }

    public void ResetSeeds()
    {
        //Reset Newt
        invSO.AddItem(database.ReferenceItem(8), 5);

        //Reset Wool
        invSO.AddItem(database.ReferenceItem(2), 5);

        //Reset Legs

        //if (invSO.InventoryItems[2].amount != 5)
        //{
        //    invSO.InventoryItems[2].amount = 5;
        //}
        //woolSeeds = invSO.InventoryItems[2].amount;

    }

    //EVERYTHING IS FISH

    public void AddFish(int _amount)
    {
        if (invSO.InventoryItems.Any(item => item.item.itemName == "IceFish"))
        {
            fish = invSO.InventoryItems[2].amount + _amount;
            invSO.InventoryItems[2].amount = fish;
        }
        else
        {
            invSO.AddItem(fishRef, _amount);
        }
        

    }

    public void ResetInv()
    {
        //if (invSO.InventoryItems[2].amount != 0)
        //{
        //    invSO.InventoryItems.RemoveAt(1);
        //}

        invSO.InventoryItems.Clear();
        ResetSeeds();

        fish = 0;
    }

}
