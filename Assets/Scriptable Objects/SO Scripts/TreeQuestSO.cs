using UnityEngine;

[CreateAssetMenu(fileName = "TreeQuestSO", menuName = "Scriptable Objects/Tree Quest")]
public class TreeQuestSO : ScriptableObject
{
    // how many total items are needed to fulfill this quest (add every number in numberRequired)
    public int questGoal;

    // tree quests can demand up to 6 different items, and any number of each of them
    // this int counts how many kinds of items it needs
    public int numOfItems;

    // the items needed and how many of each of them (should be made 1 to 1)
    public ItemSO[] questItems;
    public int[] numberRequired;

}