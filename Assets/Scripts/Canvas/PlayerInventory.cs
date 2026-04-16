using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInventory: MonoBehaviour
{
    public InventorySO inventory;
    public w_Slot[] InventorySlots;
    public ItemSO itemRef;

    //Item Description
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemDescription;

    private void OnEnable()
    {
        DisplayInventory();

        itemImage.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
    }

    // make it so items can be dragged

    public void DisplayInventory()
    {
        // Handles inventory slots
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            //Clears entire inventory
            InventorySlots[i].ResetSlot();

            // Sets slots to items in inventory
            for (int j = 0; j < inventory.InventoryItems.Count; j++)
            {
                InventoryItem curItem = inventory.InventoryItems[j];
                InventorySlots[j].SetSlot(curItem);
            }

        }
    }

    public void DisplayInfo(InventoryItem hoveredItem)
    {
        itemImage.sprite = hoveredItem.item.itemSprite;
        itemName.text = hoveredItem.item.itemName;
        itemDescription.text = hoveredItem.item.itemDescription;
    }



 

}