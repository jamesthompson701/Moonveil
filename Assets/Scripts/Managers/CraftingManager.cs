using NUnit.Framework;
using UnityEngine;

//The crafting manager
//Has a variable for the current recipe being used
//Has functions that work with inventory manager to add/remove appropriate items
public class CraftingManager : MonoBehaviour
{
    //references
    public RecipeSO curRecipe;
    public static CraftingManager instance;

    //used during crafting to check if the inventory has the neccessary items
    private bool hasItem1;
    private bool hasItem2;
    private bool hasItem3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CraftFromInventory()
    {
        if (curRecipe != null)
        {
            //checks every item in the player's inventory to see which of the ingredients the player has
            foreach (InventoryItem _item in InventoryManager.instance.invSO.InventoryItems)
            {
                if (_item.item == curRecipe.ingr1)
                {
                    hasItem1 = true;
                }
                if (_item.item == curRecipe.ingr2)
                {
                    hasItem2 = true;
                }
                if (_item.item == curRecipe.ingr3)
                {
                    hasItem3 = true;
                }
            }

            //if the player has all three ingredients, take one of each and give them the recipe output
            if (hasItem1 && hasItem2 && hasItem3)
            {
                //tutorial
                if (TutorialManager.instance != null && !TutorialManager.instance.crafting)
                {
                    //completes billboard 9: craft
                    TutorialManager.instance.ProgressTutorial(9);
                    TutorialManager.instance.crafting = true;
                }

                InventoryManager.instance.invSO.RemoveItem(curRecipe.ingr1, -1);
                InventoryManager.instance.invSO.RemoveItem(curRecipe.ingr2, -1);
                InventoryManager.instance.invSO.RemoveItem(curRecipe.ingr3, -1);
                InventoryManager.instance.invSO.AddItem(curRecipe.output, 1);

                //reset the bools
                hasItem1 = false;
                hasItem2 = false;
                hasItem3 = false;

                WorkbenchUI.instance.SuccessfulCraft();
                Debug.Log("Item Crafted");
            }
            else
            {
                WorkbenchUI.instance.FailedCraft();
                Debug.Log("Insufficient Materials");
            }
        }
        else
        {
            Debug.Log("No recipe selected");
        }

    }
    
}
