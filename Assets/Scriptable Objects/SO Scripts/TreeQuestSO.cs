using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;

[CreateAssetMenu(fileName = "TreeQuestSO", menuName = "Scriptable Objects/Tree Quest")]
public class TreeQuestSO : ScriptableObject
{
    // how many total items are needed to fulfill this quest (add every number in numberRequired)
    public int questGoal;

    // recipe unlocked for completing quest
    public RecipeSO questReward;

    // the items needed and how many of each of them (should be made 1 to 1)
    public List<ItemSO> questItems = new List<ItemSO>();
    public List<int> numberRequired = new List<int>();


    public void QuestComplete()
    {
        WorkbenchUI.instance.UnlockRecipe(questReward);
    }
}
