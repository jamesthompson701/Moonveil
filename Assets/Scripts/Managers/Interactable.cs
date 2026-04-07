using JetBrains.Annotations;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Interactable : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    //reference to soil script
    private SoilObject soil;

    //for testing purposes
    public RecipeSO trailMix;

    //variables for dispensers
    public ItemSO dispenserItem;
    public int dispenseAmount;
    public bool destroyOnDispense;

    // This method will be called by our ClickSelector
    public virtual void OnInteract()
    {
        if (gameObject.CompareTag("Soil"))                 
        {
            //reference to soil being clicked
            soil = gameObject.GetComponent<SoilObject>();
            if (PlayerInventory.instance.CheckSeeds() > 0)
            {
                //make sure soil is empty and tilled before removing a seed and spawning the crop
                if (soil.GetComponent<SoilObject>().soilContent == SoilContent.empty && soil.tilled)
                {
                    soil.SetPlantType(PlayerInventory.instance.seedRef);
                    soil.SpawnCrop();
                    PlayerInventory.instance.invSO.RemoveItem(PlayerInventory.instance.seedRef, -1);
                    Debug.Log("Seed Planted");
                }
                else
                {
                    Debug.Log("Not ready for planting, Tilled status: " + soil.tilled + ", Soil Content: " + soil.GetComponent<SoilObject>().soilContent);
                }

            }
            else
            {
                Debug.Log("Out of seeds");
            }

        }
        else if(gameObject.CompareTag("Dock"))
        {
            FishingManager.Instance.EnterFishingMode(FishingManager.Instance.currentArea);
        }
        else if (gameObject.CompareTag("Crafting"))
        {
            CanvasManager.Instance.OpenWorkbench();
        }
        else if (gameObject.CompareTag("FastTravel"))
        {
            CanvasManager.Instance.OpenFastTravel();
        }
        else if (gameObject.CompareTag("Dialogue"))
        {
            Debug.Log("Talking");
            foreach (InventoryItem _item in PlayerInventory.instance.invSO.InventoryItems)
            {
                Debug.Log("Item Name: " +  _item.item.name);
                if (_item.item.name == "ChumBowl")
                {
                    Debug.Log("Setting ChumBowl to" + _item.amount);
                    DialogueLua.SetVariable("McGuffinNum", _item.amount);
                }
            }
            DialogueManager.StartConversation("New Conversation 1");
        }
        else if (gameObject.CompareTag("Mineable"))
        {
            this.gameObject.GetComponent<MineRock>().Interact();
        }
        else
        {
            Debug.Log("Nothing interactable hit, tag is: " + gameObject.tag);
        }

        //add an item to inventory when clicked, must be set in editor
        if (gameObject.CompareTag("Dispenser"))
        {
            PlayerInventory.instance.invSO.AddItem(dispenserItem, dispenseAmount);
            if (destroyOnDispense)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Optional: A method to reset the color

}
