using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Create New Inventory")]
public class InventorySO : ScriptableObject
{
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();
    public int maxItems;
    public event Action<ItemSO, int, bool> GetInventoryItem;
    public event Action<int> AddInventoryItem;

    //TODO| Account for removing items

    public void AddItem(ItemSO newItem, int newAmount)
    {
        GetInventoryItem?.Invoke(newItem, newAmount, true);

        //Check if item is in inventory for stacking
        if (newItem.isStackable)
        {
            foreach (InventoryItem item in InventoryItems)
            {
                if (item.item == newItem)
                {
                    item.AddAmount(newAmount);
                    GetInventoryItem?.Invoke(newItem, newAmount, false);
                    //AddInventoryItem?.Invoke(newAmount);
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

