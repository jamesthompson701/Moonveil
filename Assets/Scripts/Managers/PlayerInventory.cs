using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlayerInventory : MonoBehaviour
{
    //this might be a temporary script
    //just tracks Supposed Cactus seeds
    public static PlayerInventory instance;
    public TMP_Text seedCount;

    public int seeds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        UpdateSeeds();
    }

    public void AddSeeds(int _amount)
    {
        seeds = seeds + _amount;
    }

    public int CheckSeeds()
    {
        return seeds;
    }

    public void UpdateSeeds()
    {
        seedCount.text = "" + seeds;
    }

}
