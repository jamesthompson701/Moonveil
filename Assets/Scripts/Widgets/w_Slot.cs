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

        // Update HUD display
        HUD.instance.DisplaySelectedItem(item.item, item.amount);
        HUD.instance.UpdateDisplayedItemAmount(this, item.amount);

        // If the clicked item is a seed, set the InventoryManager's seedRef so planting logic can use it.
        SeedItemSO clickedSeed = item.item as SeedItemSO;
        if (clickedSeed != null)
        {
            InventoryManager.instance.seedRef = clickedSeed;
            Debug.Log("Selected seed: " + clickedSeed.itemName);
        }
        else
        {
            // Clear seedRef when a non-seed is selected
            InventoryManager.instance.seedRef = null;
        }
    }
}
