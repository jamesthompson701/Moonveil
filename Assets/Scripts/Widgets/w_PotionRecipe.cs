using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class w_PotionRecipe : MonoBehaviour
{
 
    public Image image;
    public TMP_Text potionName;
    public TMP_Text ingr1;
    public TMP_Text ingr2;
    public TMP_Text ingr3;


    public void SetRecipe(RecipeSO recipe)
    {
        image.sprite = recipe.output.itemSprite;
        potionName.text = recipe.name;
        ingr1.text = recipe.ingr1.itemName;
        ingr2.text = recipe.ingr2.itemName;
        ingr3.text = recipe.ingr3.itemName;
    }
}
