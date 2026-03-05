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
        DisplayInventory();

        itemImage.sprite = null;
        itemDescription.text = "";
    }


    public void DisplayInventory()
    {
        
        //Handles inventory slots
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            Debug.Log("Number of items in " + i + " slot: " + InventorySlots[i].item.amount);
            if (InventorySlots[i].item.amount == 0)
            {
                InventorySlots[i].ResetSlot();
                
            }
            //Handles items in inventorySO
            for (int j = 0; j < inventory.InventoryItems.Count; j++)
            {
                
                //Assigns item to slot
                InventoryItem curItem = inventory.InventoryItems[j];
                InventorySlots[j].SetSlot(curItem);

            }

        }
    }

    public void DisplayInfo(InventoryItem hoveredItem)
    {
        itemImage.sprite = hoveredItem.item.itemSprite;
        itemDescription.text = hoveredItem.item.itemDescription;
    }

 

}
