using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class PlayerInventory : MonoBehaviour
{
    //just tracks seeds
    public static PlayerInventory instance;
    public InventorySO invSO;

    public SoilObject soilRef;

    public static int newtSeeds;
    public static int woolSeeds;
    public static int fish;

    public SeedItemSO seedRef;
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

    public void AddSeeds(int _amount, SeedItemSO _type)
    {
        //two kinds  Seeds
        switch (_type.itemName)
        {
            case "Eye Of Newt Seed":
                newtSeeds = invSO.InventoryItems[0].amount + _amount;
                invSO.InventoryItems[0].amount = newtSeeds;
                break;
            case "Wool Of Bat Seed":
                woolSeeds = invSO.InventoryItems[1].amount + _amount;
                invSO.InventoryItems[1].amount = woolSeeds;
                break;
        }
           

    }


    public int CheckSeeds()
    {
        if (seedRef.itemName == "Eye Of Newt Seed")
        {
            return newtSeeds;
        }
        else
        {
            return woolSeeds;
        }
    }

    public void ResetSeeds()
    {
        //Reset Newt
            if (invSO.InventoryItems[0].amount != 5)
            {
                invSO.InventoryItems[0].amount = 5;
            }
            newtSeeds = invSO.InventoryItems[0].amount;

        //Reset Wool
 
            if (invSO.InventoryItems[1].amount != 5)
            {
                invSO.InventoryItems[1].amount = 5;
            }
            woolSeeds = invSO.InventoryItems[1].amount;
        


           

   
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
        if (invSO.InventoryItems[2].amount != 0)
        {
            invSO.InventoryItems.RemoveAt(1);
        }

        fish = 0;
    }

}
