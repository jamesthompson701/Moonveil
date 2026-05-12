using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class BookMenu : MonoBehaviour
{
    [Header("Content Screens")]
    public GameObject[] contentScreens;
    public GameObject mainScreen;

    [Header("Recipes Tab")]
    public GameObject recipeSlot;
    public GameObject recipesTab;
    public Transform recipeGroup;
    List<GameObject> unlockedRecipes;


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

    // SETTINGS MENU
    public void DisplaySettings()
    {
        CanvasManager.Instance.OpenMenu(7);
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
        //for (int i = 0; i < recipeSlots.Length; i++)
        //{
        //    recipeSlots[i].SetRecipe(tempRecipe);
        //}

        foreach (GameObject _recipe in WorkbenchUI.instance.unlockedRecipes)
        {
            GameObject curRecipe = Instantiate(recipeSlot, recipeGroup);
            unlockedRecipes.Add(curRecipe);
            w_PotionRecipe spawnedSlot = curRecipe.GetComponent<w_PotionRecipe>();

            wRecipe recipe = _recipe.GetComponent<wRecipe>();
            spawnedSlot.SetRecipe(recipe.myRecipe);

        }
    }

    public void OnFastTravelClicked()
    {
        Debug.Log("Fast Travel Clicked");
        EnvironmentManager.Instance.Travel(eFastTravel.home);
    }
}
