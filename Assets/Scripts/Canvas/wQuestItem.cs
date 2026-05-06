using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class wQuestItem : MonoBehaviour
{
    // is set by world tree
    public ItemSO myItem;
    
    // visual elements
    public Image myImage;
    public TMP_Text myName;

    // world tree that commands this widget
    public WorldTree myWorldTree;

    private void Awake()
    {
        myImage.sprite = myItem.itemSprite;
        myName.text = myItem.itemName;
    }

    public void Clicked()
    {
        myWorldTree.ItemClicked(myItem);
    }

    public void Refresh()
    {
        myImage.sprite = myItem.itemSprite;
    }
}
