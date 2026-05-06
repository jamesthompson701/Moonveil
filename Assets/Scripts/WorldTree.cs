using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;

public class WorldTree : MonoBehaviour
{
    //menu
    public GameObject myMenu;

    //current quest
    private TreeQuestSO curQuest;
    private int curQuestInt;

    //progress on each item
    public List<int> progressTracker = new List<int>();

    //list of quests
    public List<TreeQuestSO> quests = new List<TreeQuestSO>();

    // currently chosen item + level reward + current tree level
    public Image currentlySelectedImage;
    public ItemSO currentlySelected;
    public Image levelUpReward;
    public int treeLevel;
    public TMP_Text levelText;

    //everything to do with the quest item widgets
    public GameObject questItemWidget;
    public GameObject itemsMenu;
    public List<GameObject> listOfWidgets = new List<GameObject>();

    public void Awake()
    {
        curQuestInt = 0;
        levelText.text = "" + treeLevel;
        SetupQuest();
    }

    public void OnInteract()
    {
        Debug.Log("ineracted with tree working question?");
        //test
        CanvasManager.Instance.OpenTreeMenu(myMenu);
    }

    public void ItemClicked(ItemSO _item)
    {
        currentlySelectedImage.sprite = _item.itemSprite;
        currentlySelected = _item;
    }

    public void ItemDeposited()
    {
        int completionTally = 0;
        //check each item in the quest list to see if it matches the currently selected item
        for (int i = 0; i < curQuest.questItems.Count; i++)
        {
            if (curQuest.questItems[i] == currentlySelected)
            {
                //if it's a match, check if this item is already completed
                if (progressTracker[i] == curQuest.numberRequired[i])
                {
                    completionTally++;

                    //if the item's complete, check if all of them are
                    if (completionTally == curQuest.questItems.Count)
                    {
                        // stuff to complete the quest and prepare for the next
                        curQuest.QuestComplete();
                        foreach(GameObject _widget in listOfWidgets)
                        {
                            Destroy(_widget);
                        }

                        progressTracker.Clear();

                        if (curQuestInt < quests.Count)
                        {
                            curQuestInt++;
                        }

                        treeLevel++;
                        levelText.text = "" + treeLevel;

                        SetupQuest();
                    }
                }
                else
                {
                    //scroll through the inventory and if they have the relevant item, remove 1 and add to the progress tracker
                    foreach (InventoryItem _item in InventoryManager.instance.invSO.InventoryItems)
                    {
                        if (_item.item == currentlySelected)
                        {
                            InventoryManager.instance.invSO.RemoveItem(currentlySelected, -1);
                            progressTracker[i]++;
                        }
                    }

                }

            }

        }
    }

    public void SetupQuest()
    {
        curQuest = quests[curQuestInt];

        for (int i = 0; i < curQuest.questItems.Count; i++)
        {
            //set up the widgets and progress tracker
            Debug.Log("widget gen");
            GenerateQuestItemWidget(curQuest.questItems[i]);
            progressTracker.Add(0);
        }

        //refresh the tree level
    }

    public void GenerateQuestItemWidget(ItemSO _item)
    {
        GameObject questWidget = Instantiate(questItemWidget);
        wQuestItem questItem = questWidget.GetComponent<wQuestItem>();
        questItem.myWorldTree = this;

        questWidget.transform.SetParent(itemsMenu.transform);
        questItem.myItem = _item;
        questItem.myImage.sprite = _item.itemSprite;
        questItem.Refresh();
        listOfWidgets.Add(questWidget);
    }
}
