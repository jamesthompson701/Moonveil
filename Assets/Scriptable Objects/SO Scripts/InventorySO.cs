using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Create New Inventory")]
public class InventorySO : ScriptableObject
{
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();
    public int maxItems;
    public int dropMultiplier;
    public int fishMultiplier;
    public int combatMultiplier;
    public int miningMultiplier;
    public event Action<ItemSO, int, bool> GetInventoryItem;
    public event Action<int> AddInventoryItem;

    public void AddItem(ItemSO newItem, int newAmount)
    {
        //tutorial
        if (!InventoryManager.instance.tutorialDone)
        {
            //complete billboard 2; go forage
            if(TutorialManager.instance.currentBillboard == 1)
            {
                //Item id 1 is the wool of bat seed
                if(newItem.itemID == 1)
                {
                    TutorialManager.instance.ProgressTutorial(2);
                    InventoryManager.instance.tutorialDone = true;
                }
                
            }

        }

        GetInventoryItem?.Invoke(newItem, newAmount, true);

        //Check if item is in inventory for stacking
        if (newItem.isStackable)
        {
            foreach (InventoryItem item in InventoryItems)
            {
                if (item.item == newItem)
                {
                    //Fish Multiplier
                    if (newItem.itemID >= 8 && newItem.itemID <= 10)
                    {
                        Debug.Log("Added item in Fish category with item ID: " + newItem.itemID);
                        item.AddAmount(newAmount * fishMultiplier);
                    }
                    //Combat Multiplier
                    else if (newItem.itemID == 11 || newItem.itemID == 43)
                    {
                        Debug.Log("Added item in Combat category with item ID: " + newItem.itemID);
                        item.AddAmount(newAmount * combatMultiplier);
                    }
                    //Mining Multiplier
                    else if (newItem.itemID >= 48 && newItem.itemID <= 50)
                    {
                        Debug.Log("Added item in Mining category with item ID: " + newItem.itemID);
                        item.AddAmount(newAmount * miningMultiplier);
                    }
                    else
                    {
                        Debug.Log("Added item not in mult category with item ID: " + newItem.itemID);
                        item.AddAmount(newAmount * dropMultiplier);
                    }
                    //GetInventoryItem?.Invoke(newItem, newAmount, false);
                    //AddInventoryItem?.Invoke(newAmount);
                    return;
                }
            }
        }

        //Adds item to inventory item list if not stackable OR if not already in inventory
        if (InventoryItems.Count < maxItems)
        {
            //Fish Multiplier
            if (newItem.itemID >= 8 && newItem.itemID <= 10)
            {
                Debug.Log("Added item in Fish category with item ID: " + newItem.itemID);
                InventoryItems.Add(new InventoryItem(newItem, newAmount * fishMultiplier));
            }
            //Combat Multiplier
            else if (newItem.itemID == 11 || newItem.itemID == 43)
            {
                Debug.Log("Added item in Combat category with item ID: " + newItem.itemID);
                InventoryItems.Add(new InventoryItem(newItem, newAmount * combatMultiplier));
            }
            //Mining Multiplier
            else if (newItem.itemID >= 48 && newItem.itemID <= 50)
            {
                Debug.Log("Added item in Mining category with item ID: " + newItem.itemID);
                InventoryItems.Add(new InventoryItem(newItem, newAmount * miningMultiplier));
            }
            else
            {
                Debug.Log("Added item not in mult category with item ID: " + newItem.itemID);
                InventoryItems.Add(new InventoryItem(newItem, newAmount * dropMultiplier));
            }


        }

    }

    public void RemoveItem(ItemSO newItem, int newAmount)
    {
        foreach (InventoryItem item in InventoryItems)
        {
            if (item.item == newItem)
            {
                item.AddAmount(newAmount);
                if (item.amount == 0)
                {
                    InventoryItems.Remove(item);
                }
                break;
            }
        }
    }
}



//Holds item and amount
[System.Serializable]
public class InventoryItem
{
    public ItemSO item;
    public int amount;

    //Constructor
    public InventoryItem(ItemSO _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    //Add to stack of items
    public void AddAmount(int value)
    {
        amount+= value;
    }
}

