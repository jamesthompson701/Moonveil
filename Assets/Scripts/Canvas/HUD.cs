using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    GameObject playerRef;
    SpellManager2 attackManagerRef;

    public List<GameObject> itemPopups;
    
    public GameObject[] highlight;

    public GameObject slot;
    public Transform popupGroup;

    public GameObject[] SpellChargeIcons;



    private void Awake()
    {
        playerRef = GameObject.Find("Player");
        attackManagerRef = playerRef.GetComponent<SpellManager2>();

        InventoryManager.instance.invSO.GetInventoryItem += InstantiatePopup;

        if (instance == null)
        {
            instance = this;
        }


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

    /*
    //float fireManaPercent, float earthManaPercent, float waterManaPercent, float airManaPercent
    public void UpdateManaDisplay(float[] fillPercents)
    {
        Debug.Log("Updating fire mana UI amount to " + fillPercents[0]);

        for (int i = 0; i < highlight.Length; i++)
        {
            Debug.Log
            highlight[i].GetComponent<Image>().fillAmount = fillPercents[i];
        }
    }
    */

    public void UpdateManaDisplay(float[] fillPercents)
    {
        Debug.Log("UpdateManaDisplay CALLED");

        Debug.Log("fillPercents is " + (fillPercents == null ? "NULL" : "NOT NULL"));
        Debug.Log("highlight is " + (highlight == null ? "NULL" : "NOT NULL"));

        if (fillPercents != null)
            Debug.Log("fillPercents length = " + fillPercents.Length);

        if (highlight != null)
            Debug.Log("highlight length = " + highlight.Length);

        for (int i = 0; i < highlight.Length; i++)
        {
            Debug.Log("Loop index = " + i);

            Debug.Log("highlight[" + i + "] = " + (highlight[i] != null ? highlight[i].name : "NULL"));

            var img = highlight[i].GetComponent<UnityEngine.UI.Image>();
            Debug.Log("Image component on highlight[" + i + "] = " + (img != null ? "FOUND" : "MISSING"));

            Debug.Log("Setting fillAmount to " + fillPercents[i]);

            highlight[i].GetComponent<UnityEngine.UI.Image>().fillAmount = fillPercents[i];
        }
    }

    public void UpdatedSpellCharge(int tier)
    {
        switch(tier)
        {
            case 0:
                foreach (GameObject item in SpellChargeIcons)
                {
                    item.SetActive(false);
                }
                break;
            case 1:
                SpellChargeIcons[0].SetActive(true);
                break;
            case 2:
                SpellChargeIcons[1].SetActive(true);
                break;
            case 3:
                SpellChargeIcons[2].SetActive(true);
                break;
            case 4:
                SpellChargeIcons[3].SetActive(true);
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
        // Prevents another Pop Up from spawning if not new
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

            StartCoroutine(InventoryManager.instance.DestroyPopup(popUp));

        }
    }

}
