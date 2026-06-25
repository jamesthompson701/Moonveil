using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class wRecipe : MonoBehaviour
{
    //recipe has to be set manually if you dont instantiate it
    public RecipeSO myRecipe;
    public Image myImage;

    private void Awake()
    {
        myImage.sprite = myRecipe.output.itemSprite;
    }
    public void Clicked()
    {
        WorkbenchUI.instance.RecipeClicked(myRecipe);
    }
    public void Refresh()
    {
        myImage.sprite = myRecipe.output.itemSprite;
    }
}
