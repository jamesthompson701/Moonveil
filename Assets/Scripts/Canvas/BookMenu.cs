using UnityEngine;

public class BookMenu : MonoBehaviour
{
    [Header("Content Screens")]
    public GameObject[] contentScreens;
    public GameObject mainScreen;

    [Header("Potion Recipes Tab")]
    public w_PotionRecipe[] recipeSlots;
    public GameObject potionRecipesTab;



    //DELETE THIS LATER
    public RecipeSO tempRecipe;

    private void OnEnable()
    {
        // set screen to default
        foreach (var item in contentScreens)
        {
            item.gameObject.SetActive(false);
        }
        mainScreen.SetActive(true);

        DisplayRecipes();
    }

    // capture gameplay screen and display on book
    public void DisplayGameplay()
    {

    }

    // cleaner disabling
    public void DisplayContent(GameObject _screen)
    {
        foreach (var item in contentScreens)
        {
            item.gameObject.SetActive(false);
            if (item.name == _screen.name)
            {
                item.gameObject.SetActive(true);
            }

        }
    }

    // for each recipe in recipe list instantiate a new page prefab
    public void DisplayRecipes()
    {
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            recipeSlots[i].SetRecipe(tempRecipe);
        }
    }

    public void OnFastTravelClicked()
    {
        Debug.Log("Fast Travel Clicked");
        EnvironmentManager.Instance.Travel(eFastTravel.home);
    }
}
