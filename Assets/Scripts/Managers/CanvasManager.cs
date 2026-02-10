using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public GameObject menuCanvas;
    bool isActive = false;

 
    InputAction openInv;


    public void OpenInventory()
    {

        if (!isActive)
        {
            isActive = true;
            inventoryCanvas.SetActive(true);

        }
        else
        {
            isActive = false;
            inventoryCanvas.SetActive(false);
        }
    }

    public void OpenMainMenu()
    {
        if (!isActive)
        {
            isActive = true;
            menuCanvas.SetActive(true);
        }
        else
        {
            isActive = false;
            menuCanvas.SetActive(false);
        }
    }


    private void Awake()
    {
        openInv = InputSystem.actions.FindAction("Open Inventory");
    }

    private void Update()
    {
        float inv = openInv.ReadValue<float>();
        if (inv == 1)
        {
            OpenInventory();
        }
    }


}
