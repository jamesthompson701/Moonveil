using UnityEngine;

public class SelectionWheel : MonoBehaviour

{

    public SeedItemSO newtSeed;
    public SeedItemSO woolSeed;
    public SeedItemSO lizardSeed;


    public void SelectNewt()
    {
        PlayerInventory.instance.curSeed = newtSeed;
        Debug.Log("EYE OF NEWT SEEDS SELECTED");

    }
    public void SelectWool()
    {
        PlayerInventory.instance.curSeed = woolSeed;
        Debug.Log("WOOL OF BAT SEEDS SELECTED");
    }
    public void SelectLizard()
    {
        PlayerInventory.instance.curSeed = lizardSeed;
        Debug.Log("LIZARD'S LEGS SEEDS SELECTED");
    }

}
