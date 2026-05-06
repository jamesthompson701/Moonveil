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

    //is set bool
    private bool isSet;

    //current quest
    private TreeQuestSO curQuest;
    private int curQuestInt;

    //progress on each item and total completion
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

    public void OnInteract()
    {
        CanvasManager.Instance.OpenTreeMenu(myMenu);

        if (!isSet)
        {
            curQuestInt = 0;
            levelText.text = "" + treeLevel;
            SetupQuest();
            //levelText.transform.position = new Vector3(-738, 6, -533);
            isSet = true;
        }
    }

    public void ItemClicked(ItemSO _item)
    {
        currentlySelectedImage.sprite = _item.itemSprite;
        currentlySelected = _item;
    }

    public void ItemDeposited()
    {
        //check each item in the quest list to see if it matches the currently selected item
        for (int i = 0; i < curQuest.questItems.Count; i++)
        {
            if (curQuest.questItems[i] == currentlySelected)
            {
                //if it's a match, check if this item is already completed
                if (progressTracker[i] == curQuest.numberRequired[i])
                {
                    Debug.Log("Item number fulfilled!!!!!!!!!!!!");
                    progressTracker[i] = 144;
                    int completeCount = 0;

                    //if the item's complete, check if all of them are
                    foreach(int _num in progressTracker)
                    {
                        if(_num == 144)
                        {
                            completeCount++;
                            if(completeCount == progressTracker.Count)
                            {
                                Debug.Log("WHOLE QUEST complete");

                                // stuff to complete the quest and prepare for the next
                                curQuest.QuestComplete();
                                foreach (GameObject _widget in listOfWidgets)
                                {
                                    Destroy(_widget.gameObject);
                                }

                                listOfWidgets.Clear();
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
                    }
                    
                }
                else
                {
                    //scroll through the inventory and if they have the relevant item, remove 1 and add to the progress tracker
                    foreach (InventoryItem _item in InventoryManager.instance.invSO.InventoryItems)
                    {
                        if (_item.item == currentlySelected)
                        {
                            if(progressTracker[i] != 144)
                            {
                                Debug.Log(_item.item.itemName + " added");
                                InventoryManager.instance.invSO.RemoveItem(currentlySelected, -1);
                                progressTracker[i]++;
                            }
                            else { Debug.Log("item's full bud"); }
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
            levelUpReward.sprite = curQuest.questReward.output.itemSprite;
            
        }

        //refresh the tree level
        levelText.text = "" + treeLevel;
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
