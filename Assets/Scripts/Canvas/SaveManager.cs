using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
        Debug.Log("Instance is: " + Instance);
    }

    public void SaveInventory()
    {
        Debug.Log("Save Inventory");
        string itemInventory = "";
        foreach (InventoryItem item in InventoryManager.instance.invSO.InventoryItems)
        {
            // Pull the amount and item ID from the list
            itemInventory += item.amount + "," + item.item.itemID + ";";
        }
        PlayerPrefs.SetString("inventory", itemInventory);
    }
    
    public void RestoreInventory()
    {
        Debug.Log("Restore Inventory");

        InventoryRepository repo = InventoryManager.instance.inventoryRepository;

        InventoryManager.instance.ResetInv();

        string itemInventory = PlayerPrefs.GetString("inventory");
        Debug.Log("Inventory String " + itemInventory);
        string[] str = itemInventory.Split(new string[] { ";"}, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < str.Length; i++)
        {
            string[] subStr = str[i].Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            int amount = Convert.ToInt32(subStr[0]);
            Debug.Log("Amount = " + amount);
            int itemID = Convert.ToInt32(subStr[1]);
            Debug.Log("Item ID = " + itemID);

            InventoryManager.instance.invSO.AddItem(repo.items[itemID], amount);
        }
    }
}