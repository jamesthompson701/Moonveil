using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class w_SelectionWheel : MonoBehaviour
{
    public SeedItemSO seed;

    public Toggle toggle;
    public bool isPressed = false;

    CanvasManager managerRef;

    private void Awake()
    {
        GameObject managerObj = GameObject.Find("CanvasManager");
        managerRef = managerObj.GetComponent<CanvasManager>();
    }

    public void SelectSeed()
    {
        PlayerInventory.instance.seedRef = seed;
        managerRef.OpenSelectionWheel();
    }


}
