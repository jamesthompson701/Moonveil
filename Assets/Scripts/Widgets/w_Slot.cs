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

    public InventoryManager managerRef;

    //Finds inventory manager
    private void Awake()
    {
        GameObject managerObj = GameObject.Find("PlayerInventoryUI");
        managerRef = managerObj.GetComponent<InventoryManager>();
    }

    //Sets image and amount to items data
    public void SetItem(InventoryItem _item)
    {
        button.SetActive(true);
        image.sprite = _item.item.itemSprite;
        amount.text = "" + _item.amount;
        item = _item;
    }


    //Displays information when hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.activeInHierarchy)
        {
            managerRef.DisplayInfo(item);
        }

    }

}
