using UnityEngine;

public class BookMenu : MonoBehaviour
{
    [Header("Potion Recipes Tab")]
    public w_PotionRecipe[] recipeSlots;
    public GameObject potionRecipesTab;

    //DELETE THIS LATER
    public RecipeSO tempRecipe;

    private void Awake()
    {
        DisplayRecipes();
    }



    public void DisplayRecipes()
    {
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            recipeSlots[i].SetRecipe(tempRecipe);
        }
    }
}
