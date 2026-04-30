using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorldTree : MonoBehaviour
{
    //menu
    public GameObject myMenu;

    //current quest
    private TreeQuestSO curQuest;

    //quest progress on each item
    private int item1Count;
    private int item2Count;
    private int item3Count;
    private int item4Count;
    private int item5Count;
    private int item6Count;

    //list of quests
    public TreeQuestSO[] quests;

    // currently chosen item
    public Image currentlySelectedImage;
    public ItemSO currentlySelected;


    //everything to do with the quest item widgets
    public GameObject questItemWidget;
    public GameObject itemsMenu;

    public void Awake()
    {
        curQuest = quests[0];
    }

    public virtual void OnInteract()
    {
        myMenu.SetActive(true);
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
        if (curQuest.item1 == currentlySelected)
        {
            item1Count++;
            if (item1Count == curQuest.item1Needed)
            {

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
    }
}
