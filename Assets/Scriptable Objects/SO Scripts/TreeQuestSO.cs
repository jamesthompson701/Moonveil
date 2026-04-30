using UnityEngine;

[CreateAssetMenu(fileName = "TreeQuestSO", menuName = "Scriptable Objects/Tree Quest")]
public class TreeQuestSO : ScriptableObject
{
    // how many total items are needed to fulfill this quest
    public int questGoal;

    // tree quests can demand up to 6 different items, and any number of each of them
    // this int counts how many kinds of items it needs
    public int numOfItems;

    // the items needed and how many of each of them
    public ItemSO item1;
    public int item1Needed;

    public ItemSO item2;
    public int item2Needed;

    public ItemSO item3;
    public int item3Needed;

    public ItemSO item4;
    public int item4Needed;

    public ItemSO item5;
    public int item5Needed;

    public ItemSO item6;
    public int item6Needed;
}