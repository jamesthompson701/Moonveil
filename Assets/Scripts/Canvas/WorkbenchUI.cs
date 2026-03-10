using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        Invoke("ToggleCraftFailed", 0f);
        Invoke("ToggleCraftFailed", 2f);
    }

    public void SuccessfulCraft()
    {
        Invoke("ToggleCraftSuccess", 0f);
        Invoke("ToggleCraftSuccess", 2f);
    }

    public void CraftClicked()
    {
        CraftingManager.instance.CraftFromInventory();
    }

    public void ExitCrafting()
    {
        CanvasManager.Instance.OpenWorkbench();
    }

    //chud invokeable functions
    public void ToggleCraftFailed()
    {
        if(craftFailed.activeInHierarchy)
        {
            craftFailed.SetActive(false);
        }    
        else
        {
            craftFailed.SetActive(true);
        }
    }
    public void ToggleCraftSuccess()
    {
        if (craftSuccess.activeInHierarchy)
        {
            craftSuccess.SetActive(false);
        }
        else
        {
            craftSuccess.SetActive(true);
        }
    }
}
