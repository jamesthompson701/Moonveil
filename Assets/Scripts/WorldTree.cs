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

    //progress on each item
    public List<int> progressTracker = new List<int>();

    //list of quests
    public List<TreeQuestSO> quests = new List<TreeQuestSO>();

    // currently chosen item
    public Image currentlySelectedImage;
    public ItemSO currentlySelected;


    //everything to do with the quest item widgets
    public GameObject questItemWidget;
    public GameObject itemsMenu;
    public List<GameObject> listOfWidgets = new List<GameObject>();

    public void Awake()
    {
        curQuest = quests[0];

        for (int i = 0; i < curQuest.questItems.Count; i++)
        {
            Debug.Log("widget gen");
            GenerateQuestItemWidget(curQuest.questItems[i]);
        }
    }

    public virtual void OnInteract()
    {
        CanvasManager.Instance.SetTreeMenu(myMenu);
    }

    public void ItemClicked(ItemSO _item)
    {
        currentlySelectedImage.sprite = _item.itemSprite;
        currentlySelected = _item;
    }

    public void ItemDeposited()
    {
        //when the deposit button is clicked, check each of the quest items to see which one matches the deposited item
        //increase the count for that item and, if the count matches how many are needed, delete that from the menu
        //Finally, check the item counts again. If every item on the menu is at max, move on to the next quest
        for (int i = 0; i < curQuest.questItems.Count; i++)
        {
            if (curQuest.questItems[i] == currentlySelected)
            {
                if (progressTracker[i] < 1)
                {
                    progressTracker.Add(1);
                }
                else
                {
                    progressTracker[i]++;
                }
            }
            if (progressTracker[i] == curQuest.numberRequired[i])
            {
                curQuest.QuestComplete();

            }
        }
    }

    public void GenerateQuestItemWidget(ItemSO _item)
    {
        GameObject questWidget = Instantiate(questItemWidget);
        wQuestItem questItem = questWidget.GetComponent<wQuestItem>();

        questWidget.transform.SetParent(itemsMenu.transform);
        questItem.myItem = _item;
        questItem.myImage.sprite = _item.itemSprite;
        questItem.Refresh();
        listOfWidgets.Add(questWidget);
    }
}
