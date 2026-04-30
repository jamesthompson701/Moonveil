using UnityEngine;

[CreateAssetMenu(fileName = "TreeQuestSO", menuName = "Scriptable Objects/Tree Quest")]
public class TreeQuestSO : ScriptableObject
{
    // how many total items are needed to fulfill this quest
    public int questGoal;

    // tree quests can demand up to 5 different items, and any number of each of them

    // this int counts how many kinds of items it needs (up to 5)
    public int numOfItems;

    // the items needed and how many of each of them
    public ItemSO item1;
    public int item1Count;

    public ItemSO item2;
    public int item2Count;

    public ItemSO item3;
    public int item3Count;

    public ItemSO item4;
    public int item4Count;

    public ItemSO item5;
    public int item5Count;
}