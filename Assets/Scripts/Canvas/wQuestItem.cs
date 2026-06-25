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
    public TMP_Text myProgress;

    // world tree that commands this widget
    public WorldTree myWorldTree;

    public void Clicked()
    {
        myWorldTree.ItemClicked(myItem);
    }

    public void Refresh()
    {
        myImage.sprite = myItem.itemSprite;
        myName.text = myItem.itemName;
    }

    public void Progress(int _current, int _total)
    {
        myProgress.text = _current + "/" + _total;
    }
}
