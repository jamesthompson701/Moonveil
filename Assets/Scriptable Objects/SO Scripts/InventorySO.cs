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
        //Check if item is in inventory for stacking
        if (newItem.isStackable)
        {
            foreach (InventoryItem item in InventoryItems)
            {
                if (item.item == newItem)
                {
                    item.AddAmount(newAmount);
                    break;
                }
            }
        }

        //Adds item to inventory item list if not stackable
        if (InventoryItems.Count < maxItems)
        {
            InventoryItems.Add(new InventoryItem(newItem, newAmount));
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

