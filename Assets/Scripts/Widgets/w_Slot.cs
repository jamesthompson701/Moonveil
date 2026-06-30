using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class w_Slot : MonoBehaviour, IPointerEnterHandler
{
    public InventoryItem item;
    public GameObject button;
    public Image image;
    public TMP_Text amount;
    public PlayerInventory managerRef;
    public SeedItemSO seed;

    //Finds inventory manager
    private void Awake()
    {
        GameObject managerObj = GameObject.Find("PlayerInventoryUI");
        managerRef = managerObj.GetComponent<PlayerInventory>();
    }

    //Sets image and amount to items data
    public void SetSlot(InventoryItem _item)
    {
        button.SetActive(true);
        image.sprite = _item.item.itemSprite;
        amount.text = "" + _item.amount;
        item = _item;
    }

    public void ResetSlot()
    {
        button.SetActive(false);
        amount.text = string.Empty;
        image.sprite = null;
        item = null;
    }

    //Displays information when hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.activeInHierarchy)
        {
            managerRef.DisplayInfo(item);
        }
    }

    // Handle left mouse click to select item. This determines which item is displayed on screen and if it is a seed sets it as the current SeeditemSO.
    //public void OnItemClicked()
    //{
    //    Debug.Log("You pwessed me!");
    //    HUD.instance.DisplaySelectedItem(item.item, item.amount);
    //    w_Slot thisSlot = this;
    //    HUD.instance.UpdateDisplayedItemAmount(thisSlot,  item.amount);
    //    seed.plantType =
    //    InventoryManager.instance.seedRef = 
    //}

    public void OnItemClicked()
    {
        Debug.Log("You pwessed me!");

        if (item == null || item.item == null)
        {
            Debug.LogWarning("Clicked an empty slot.");
            return;
        }

        // If item has an effect, activate it immediately
        if (item.item.effect != null)
        {
            Debug.Log("Using item effect for: " + item.item.itemName);
            item.item.effect.UseItem();

            // Remove one from inventory (match pattern used elsewhere in project)
            InventoryManager.instance.invSO.RemoveItem(item.item, -1);

            // Update local stack count and UI
            //item.amount -= 1;
            if (item.amount <= 0)
            {
                ResetSlot();
                // Ensure HUD doesn't try to display a removed slot
                if (HUD.instance.lastSelectedSlot == this)
                {
                    HUD.instance.itemDisplay.SetActive(false);
                }
            }
            else
            {
                amount.text = "" + item.amount;
                HUD.instance.UpdateDisplayedItemAmount(this, item.amount);
                HUD.instance.DisplaySelectedItem(item.item, item.amount);
            }

            // If the used item was a seed, clear seedRef because it's a use action, not a selection
            InventoryManager.instance.seedRef = item.item as SeedItemSO;
            if (InventoryManager.instance.seedRef == null)
            {
                // ensure seedRef cleared when not selecting a seed
                InventoryManager.instance.seedRef = null;
            }

            return;
        }

        // No effect: treat as selection (for display / planting)
        HUD.instance.DisplaySelectedItem(item.item, item.amount);
        HUD.instance.UpdateDisplayedItemAmount(this, item.amount);

        SeedItemSO clickedSeed = item.item as SeedItemSO;
        if (clickedSeed != null)
        {
            InventoryManager.instance.seedRef = clickedSeed;
            Debug.Log("Selected seed: " + clickedSeed.itemName);
        }
        else
        {
            InventoryManager.instance.seedRef = null;
        }
    }
}
