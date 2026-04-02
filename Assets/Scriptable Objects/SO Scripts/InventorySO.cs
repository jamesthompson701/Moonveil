using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Create New Inventory")]
public class InventorySO : ScriptableObject
{
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();
    public int maxItems;

    //TODO| Account for removing items

    public void AddItem(ItemSO newItem, int newAmount)
    {
        //tutorial
        if (!TutorialManager.instance.inventoryDone)
        {
            TutorialManager.instance.ProgressTutorial(3);
            TutorialManager.instance.inventoryDone = true;
        }
        /*
        else if (!TutorialManager.instance.craftingDone)
        {
            if (newItem.itemID == 144)
            {
                TutorialManager.instance.ProgressTutorial(12);
                TutorialManager.instance.craftingDone = true;
            }
        }
        */

        //Check if item is in inventory for stacking
        if (newItem.isStackable)
        {
            foreach (InventoryItem item in InventoryItems)
            {
                if (item.item == newItem)
                {
                    item.AddAmount(newAmount);
                    return;
                }
            }
        }

        //Adds item to inventory item list if not stackable
        if (InventoryItems.Count < maxItems)
        {
            InventoryItems.Add(new InventoryItem(newItem, newAmount));
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

