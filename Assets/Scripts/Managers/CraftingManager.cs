using UnityEngine;

//The crafting manager
//Has a variable for the current recipe being used
//Has functions that work with inventory manager to add/remove appropriate items
public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CraftFromInventory(RecipeSO _recipe)
    {
            PlayerInventory.instance.invSO.RemoveItem(_recipe.ingr1, -1);
            PlayerInventory.instance.invSO.RemoveItem(_recipe.ingr2, -1);
            PlayerInventory.instance.invSO.RemoveItem(_recipe.ingr3, -1);
            PlayerInventory.instance.invSO.AddItem(_recipe.output, 1);

    }
    
}
