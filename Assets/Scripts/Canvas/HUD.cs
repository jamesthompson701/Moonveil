using System.Collections;
using UnityEngine;

public class HUD : MonoBehaviour
{
    GameObject playerRef;
    SpellManager attackManagerRef;

    public w_ItemPopup itemPopup;
    public GameObject popupGroup;

    public GameObject[] highlight;
    public InventoryManager managerRef;



    private void Awake()
    {
        playerRef = GameObject.Find("Player");
        attackManagerRef = playerRef.GetComponent<SpellManager>();

        GameObject managerObj = GameObject.Find("PlayerInventoryUI");
        managerRef = managerObj.GetComponent<InventoryManager>();


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

        managerRef.inventory.GetInventoryItem += itemPopup.SetSlot;

    }

    public void SetActive(int index)
    {
        for (int i = 0; i < highlight.Length; i++)
        {
            highlight[i].SetActive(false);
        }
        highlight[index].SetActive(true);
    }

    public void InstantiatePopup()
    {

    }

}
