using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    public InventorySO inventory;
    public w_Slot[] InventorySlots;

    //Item Description
    public Image itemImage;
    public TMP_Text itemDescription;


    private void OnEnable()
    {
        //Resets entire Inv if no items in Inventory SO
        if (inventory.InventoryItems.Count != 0)
        {
            DisplayInventory();
        }
        else
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                InventorySlots[i].ResetSlot();
            }
         
        }



        itemImage.sprite = null;
        itemDescription.text = "";
    }


    public void DisplayInventory()
    {
        //Checks slots in Inventory UI
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            //Clears slot if item amount is 0
            if (InventorySlots[i].item.amount == 0)
            {
                InventorySlots[i].ResetSlot();
            }
                //Checks items in inventory
                for (int j = 0; j < inventory.InventoryItems.Count; j++)
                {
                    //Assigns item to slot
                    InventoryItem curItem = inventory.InventoryItems[j];
                    InventorySlots[j].SetSlot(curItem);
                }
        }
    }

    public void ResetInventory()
    {

    }

    public void DisplayInfo(InventoryItem hoveredItem)
    {
        itemImage.sprite = hoveredItem.item.itemSprite;
        itemDescription.text = hoveredItem.item.itemDescription;
    }

 

}
