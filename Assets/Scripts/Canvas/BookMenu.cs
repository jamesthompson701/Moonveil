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

    // capture gameplay screen and display on book

    // for each recipe in recipe list instantiate a new page prefab

    // cleaner disabling

    public void DisplayRecipes()
    {
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            recipeSlots[i].SetRecipe(tempRecipe);
        }
    }
}
