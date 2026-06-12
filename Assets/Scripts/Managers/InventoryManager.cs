using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    //just tracks seeds
    // ^does not, in fact, just track seeds anymore
    public static InventoryManager instance;
    public InventorySO invSO;

    public SoilObject soilRef;

    public static int fish;

    //player currency counter
    public int crescants;

    //tutorial
    public bool tutorialDone;

    public SeedItemSO seedRef;
    public ItemSO fishRef;

    public bool isMultiplierBuffActive;
    //Amount of time left in the item multiplier buff
    public float multiplierBuffTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ResetInv();
    }

    private void Update()
    {
        if (isMultiplierBuffActive)
        {
            multiplierBuffTime -= Time.deltaTime;

            if (multiplierBuffTime <= 0f)
            {
                isMultiplierBuffActive = false;
                multiplierBuffTime = 0f;
                invSO.dropMultiplier = 1;
            }
        }
    }

    //EVERYTHING IS SEEDS
    public int CheckSeeds()
    {
        foreach (var item in invSO.InventoryItems)
        {
            if (item.item.itemID == seedRef.itemID)
            {
                return item.amount;
            }
        }
        return 0;
    }

    //EVERYTHING IS FISH

    public void AddFish(FishData _fish, int _amount)
    {
        invSO.AddItem(_fish.fishItem, _amount);
    }

    public void ResetInv()
    {
        invSO.InventoryItems.Clear();
        fish = 0;
        invSO.dropMultiplier = 1;
        isMultiplierBuffActive = false;
    }

    public IEnumerator DestroyPopup(GameObject popUp)
    {
        Debug.Log("COROUTINE RAN");
        yield return new WaitForSeconds(3);
        Destroy(popUp);
        HUD.instance.itemPopups.Remove(popUp);
    }
}
