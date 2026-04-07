using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    GameObject playerRef;
    SpellManager attackManagerRef;

    public List<GameObject> itemPopups;
    
    public GameObject[] highlight;
    public InventoryManager managerRef;

    public GameObject slot;
    public Transform popupGroup;



    private void Awake()
    {
        playerRef = GameObject.Find("Player");
        attackManagerRef = playerRef.GetComponent<SpellManager>();

        managerRef = InventoryManager.instance;

        managerRef.invSO.GetInventoryItem += InstantiatePopup;


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

    public void InstantiatePopup(ItemSO _item, int _amount, bool isNew)
    {
        foreach (var popUp in itemPopups)
        {
            w_ItemPopup spawnedPopup = popUp.GetComponent<w_ItemPopup>();
            if (spawnedPopup.item == _item)
            {
                spawnedPopup.AddAmount(_amount);
                return;
            }

        }

        if (isNew)
        {
            GameObject popUp = Instantiate(slot, popupGroup);
            itemPopups.Add(popUp);
            w_ItemPopup spawnedPopup = popUp.GetComponent<w_ItemPopup>();
            spawnedPopup.SetPopup(_item, _amount);

            StartCoroutine(DestroyPopup(popUp));

        }
    }

    IEnumerator DestroyPopup(GameObject popUp)
    {
        yield return new WaitForSeconds(3);
        Destroy(popUp);
        itemPopups.Remove(popUp);
    }

}
