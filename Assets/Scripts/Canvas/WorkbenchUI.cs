using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorkbenchUI : MonoBehaviour
{
    //all the item images and names
    public Image itemImage1;
    public Image itemImage2;
    public Image itemImage3;
    public Image outputImage;

    public TMP_Text itemName1;
    public TMP_Text itemName2;
    public TMP_Text itemName3;
    public TMP_Text outputName;
    public TMP_Text outputName2;

    //list of unlocked recipes
    public List<GameObject> unlockedRecipes;

    //wRecipe prefab
    public GameObject blankRecipeWidget;

    //place to instantiate the recipes
    public GameObject recipeBox;

    //Craft failed and succeeded text
    public GameObject craftFailed;
    public GameObject craftSuccess;


    public static WorkbenchUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RecipeClicked(RecipeSO _recipe)
    {
        CraftingManager.instance.curRecipe = _recipe;

        //update all the relevent graphics
        itemImage1.sprite = CraftingManager.instance.curRecipe.ingr1.itemSprite;
        itemImage2.sprite = CraftingManager.instance.curRecipe.ingr2.itemSprite;
        itemImage3.sprite = CraftingManager.instance.curRecipe.ingr3.itemSprite;
        outputImage.sprite = CraftingManager.instance.curRecipe.output.itemSprite;

        //update all the relevent text
        itemName1.text = CraftingManager.instance.curRecipe.ingr1.itemName;
        itemName2.text = CraftingManager.instance.curRecipe.ingr2.itemName;
        itemName3.text = CraftingManager.instance.curRecipe.ingr3.itemName;
        outputName.text = CraftingManager.instance.curRecipe.output.itemName;
        outputName2.text = CraftingManager.instance.curRecipe.output.itemName;
    }

    public void FailedCraft()
    {
        Invoke("CraftFailedOn", 0f);
        Invoke("CraftFailedOff", 1f);
    }

    public void SuccessfulCraft()
    {
        Invoke("CraftSuccessOn", 0f);
        Invoke("CraftSuccessOff", 1f);
    }

    public void CraftClicked()
    {
        CraftingManager.instance.CraftFromInventory();
    }

    public void ExitCrafting()
    {
        CanvasManager.Instance.OpenWorkbench();
    }

    //function that instantiates a new recipe widget
    //called by other things
    public void UnlockRecipe(RecipeSO _recipe)
    {
        GameObject unlockedRecipe = Instantiate(blankRecipeWidget);
        wRecipe w_Recipe = unlockedRecipe.GetComponent<wRecipe>();
        unlockedRecipe.transform.SetParent(recipeBox.transform);
        w_Recipe.myRecipe = _recipe;
        w_Recipe.myImage.sprite = _recipe.output.itemSprite;
        w_Recipe.Refresh();
    }

    //chud invokeable functions
    public void CraftFailedOn()
    {
            craftFailed.SetActive(true);
    }
    public void CraftFailedOff()
    {
        craftFailed.SetActive(false);
    }

    public void CraftSuccessOn()
    {
            craftSuccess.SetActive(true);
    }
    public void CraftSuccessOff()
    {
        craftSuccess.SetActive(false);
    }
}
