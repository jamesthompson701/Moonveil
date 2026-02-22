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
        if (PlayerInventory.instance.newtSeeds >= 0 && PlayerInventory.instance.woolSeeds >= 0 && PlayerInventory.instance.lizardSeeds >= 0)
        {
            PlayerInventory.instance.AddSeeds(-1, (SeedItemSO)_recipe.ingr1);
            PlayerInventory.instance.AddSeeds(-1, (SeedItemSO)_recipe.ingr2);
            PlayerInventory.instance.AddSeeds(-1, (SeedItemSO)_recipe.ingr3);
            PlayerInventory.instance.invSO.AddItem(_recipe.output, 1);
        }

    }
    
}
