using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;

public class BookMenu : MonoBehaviour
{
    [Header("Content Screens")]
    public GameObject[] contentScreens;
    public GameObject mainScreen;
    public TMP_Text menuTitle;

    [Header("Recipes Tab")]
    public GameObject recipeSlot;
    public GameObject recipesTab;
    public Transform recipeGroup;
    public List<GameObject> recipeList;


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
        menuTitle.text = "";

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
        foreach (GameObject _recipe in recipeList)
        {
            Destroy(_recipe);
        }
        recipeList.Clear();

        if (WorkbenchUI.instance != null)
        {
  
            foreach (wRecipe _recipe in WorkbenchUI.instance.unlockedRecipes)
            {
                // tracks spawned recipes
                GameObject curRecipe = Instantiate(recipeSlot, recipeGroup);
                recipeList.Add(curRecipe);

                //gets set method
                w_PotionRecipe spawnedSlot = curRecipe.GetComponent<w_PotionRecipe>();
                spawnedSlot.SetRecipe(_recipe.myRecipe);

            }
            

        }


    }

    public void OnFastTravelClicked()
    {
        Debug.Log("Fast Travel Clicked");
        EnvironmentManager.Instance.Travel(eFastTravel.home);
    }
}
