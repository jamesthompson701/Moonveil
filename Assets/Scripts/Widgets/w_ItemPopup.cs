using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class w_ItemPopup : MonoBehaviour
{
    public Image image;
    public TMP_Text amountText;
    public int amount;
    public TMP_Text itemName;
    public ItemSO item;

    public void SetPopup(ItemSO _item, int _amount)
    {
        item = _item;
        amount = _amount;
        itemName.text = _item.itemName;
        image.enabled = true;
        image.sprite = _item.itemSprite;
        amountText.text = "" + amount;

    }

    public void AddAmount(int newAmount)
    {
        amount += newAmount;
        amountText.text = "" + amount;
    }


}
