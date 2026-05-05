using UnityEngine;

[CreateAssetMenu(fileName = "TreeQuestSO", menuName = "Scriptable Objects/Tree Quest")]
public class TreeQuestSO : ScriptableObject
{
    // how many total items are needed to fulfill this quest (add every number in numberRequired)
    public int questGoal;

    // recipe unlocked for completing quest
    public RecipeSO questReward;

    // the items needed and how many of each of them (should be made 1 to 1)
    public ItemSO[] questItems;
    public int[] numberRequired;


    public void QuestComplete()
    {
        WorkbenchUI.instance.UnlockRecipe(questReward);
    }
}