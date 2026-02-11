using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager: MonoBehaviour
{
    public InventorySO inventory;
    public w_Slot[] InventorySlots;
    public ItemSO itemRef;

    //Item Description
    public Image itemImage;
    public TMP_Text itemDescription;


    private void OnEnable()
    {
        //inventory.AddItem(itemRef, 2);
        DisplayInventory();

        itemImage.sprite = null;
        itemDescription.text = "";
    }


    public void DisplayInventory()
    {
        
        //Checks how many slots
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            //Checks items in inventory
            for (int j = 0; j < inventory.InventoryItems.Count; j++)
            {
                //Assigns item to slot
                InventoryItem curItem = inventory.InventoryItems[j];
                InventorySlots[j].SetItem(curItem);

            }

        }
    }

    public void DisplayInfo(InventoryItem hoveredItem)
    {
        itemImage.sprite = hoveredItem.item.itemSprite;
        itemDescription.text = hoveredItem.item.itemDescription;
    }

 

}
