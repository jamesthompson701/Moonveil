using System.Collections;
using UnityEngine;

public class HUD : MonoBehaviour
{
    GameObject playerRef;
    SpellManager attackManagerRef;

    public w_ItemPopup itemPopup;

    public GameObject[] highlight;
    public InventoryManager managerRef;

    public GameObject slot;
    public Transform popupGroup;


    private void Awake()
    {
        playerRef = GameObject.Find("Player");
        attackManagerRef = playerRef.GetComponent<SpellManager>();

        GameObject managerObj = GameObject.Find("PlayerInventoryUI");
        managerRef = managerObj.GetComponent<InventoryManager>();

        managerRef.inventory.GetInventoryItem += InstantiatePopup;


    }
    private void Update()
    {
        switch (attackManagerRef.attackChoice)
        {
            case 1:
                SetActive(0);
                break;
            case 2:
                SetActive(1);
                break;
            case 3:
                SetActive(2);
                break;
            case 4:
                SetActive(3);
                break;
        }



    }

    public void SetActive(int index)
    {
        for (int i = 0; i < highlight.Length; i++)
        {
            highlight[i].SetActive(false);
        }
        highlight[index].SetActive(true);
    }

    public void InstantiatePopup(ItemSO _item, int _amount)
    {
        GameObject popUp = Instantiate(slot, popupGroup); 
        w_ItemPopup spawnedPopup = popUp.GetComponent<w_ItemPopup>();
        spawnedPopup.itemName.text = _item.itemName;
        spawnedPopup.image.sprite = _item.itemSprite;
        spawnedPopup.amount.text = "" + _amount;

//managerRef.inventory.AddInventoryItem += AddPopup;


        Destroy(popUp, 3);
    }

    public void AddPopup(w_ItemPopup _popUp, int _amount)
    {

    }

}
