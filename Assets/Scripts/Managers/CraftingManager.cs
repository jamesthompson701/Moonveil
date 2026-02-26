using UnityEngine;

//The crafting manager
//Has a variable for the current recipe being used
//Has functions that work with inventory manager to add/remove appropriate items
public class CraftingManager : MonoBehaviour
{
    public RecipeSO currentRecipe;

    public static CraftingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CraftFromInventory()
    {
        
    }
    
}
