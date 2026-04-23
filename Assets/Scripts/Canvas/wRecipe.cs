using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class wRecipe : MonoBehaviour
{
    //recipe has to be set manually
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
