using UnityEngine;
using TMPro;
using UnityEngine.UI;

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


    public void RecipeClicked(RecipeSO _recipe)
    {
        CraftingManager.instance.curRecipe = _recipe;
    }

    public void CraftClicked()
    {
        CraftingManager.instance.CraftFromInventory();
    }
}
