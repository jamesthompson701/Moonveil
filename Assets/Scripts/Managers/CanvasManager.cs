using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public GameObject menuCanvas;
    bool isActive = false;

    public InputActionAsset input;
    InputAction openInv;
    InputAction openPause;

    InputActionMap player;
    InputActionMap UI;


    private void Awake()
    {
        openInv = input.FindAction("Inventory");
        openPause = input.FindAction("Pause");


        player = input.FindActionMap("Player");
        UI = input.FindActionMap("UI");


    }

    private void Update()
    {
        bool inv = openInv.WasPressedThisFrame();
        if (inv == true)
        {
            Debug.Log("ITS PRESSED");
            OpenInventory();
        }

        bool pause = openPause.WasPressedThisFrame();
        if (pause == true)
        {
            OpenPause();
        }
    }

    public void OpenInventory()
    {

        if (!isActive)
        {
            isActive = true;
            inventoryCanvas.SetActive(true);
            OpenMenu();

        }
        else
        {
            isActive = false;
            inventoryCanvas.SetActive(false);
            CloseMenu();
        }
        openInv = input.FindAction("Inventory");
    }

    public void OpenPause()
    {
        if (!isActive)
        {
            isActive = true;
            menuCanvas.SetActive(true);
            OpenMenu();

        }
        else
        {
            isActive = false;
            menuCanvas.SetActive(false);
            CloseMenu();
        }
        openPause = input.FindAction("Pause");
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.Disable();
        UI.Enable();
 

    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.Enable();
        UI.Disable();
        openInv = input.FindAction("Inventory");
    }



}
