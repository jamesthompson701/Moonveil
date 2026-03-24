using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class w_ItemPopup : MonoBehaviour
{
    public InventoryItem item;
    public Image image;
    public TMP_Text amount;
    public TMP_Text itemName;

    public GameObject slot;

    //Sets image and amount to items data
    public void SetSlot(ItemSO _item, int _amount)
    {
        Instantiate(slot); //fix this later
        image.sprite = _item.itemSprite;
        amount.text = "" + _amount;
        itemName.text = _item.itemName;
    }


}
