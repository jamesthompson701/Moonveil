using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public GameObject menuCanvas;
    public GameObject fastTravelCanvas;
    public GameObject selectionCanvas;
    public GameObject HUD;
    public GameObject workbenchCanvas;
    bool isActive = false;

    public InputActionAsset input;
    InputAction openInv;
    InputAction openPause;
    InputAction openSelection;

    InputActionMap player;
    InputActionMap UI;

    public static CanvasManager Instance;


    private void Awake()
    {

        openInv = input.FindAction("Inventory");
        openPause = input.FindAction("Pause");
        openSelection = input.FindAction("Selection");
        


        player = input.FindActionMap("Player");
        UI = input.FindActionMap("UI");

        //Making canvas manager a singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

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

        bool selection = openSelection.WasPressedThisFrame();
        if (selection == true)
        {
            OpenSelectionWheel();
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

    public void OpenFastTravel()
    {

        if (!isActive)
        {
            isActive = true;
            fastTravelCanvas.SetActive(true);
            OpenMenu();

        }
        else
        {
            isActive = false;
            fastTravelCanvas.SetActive(false);
            CloseMenu();
        }
    }

    public void OpenWorkbench()
    {
        if(!isActive)
        {
            isActive = true;
            workbenchCanvas.SetActive(true);
            OpenMenu();
        }
        else
        {
            isActive = false;
            workbenchCanvas.SetActive(false);
            CloseMenu();
        }
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

    public void OpenSelectionWheel()
    {
        if (!isActive)
        {
            isActive = true;
            selectionCanvas.SetActive(true);
            OpenMenu();

        }
        else
        {
            isActive = false;
            selectionCanvas.SetActive(false);
            CloseMenu();

        }
        openSelection = input.FindAction("Selection");
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;

        player.Disable();
        UI.Enable();

        HUD.SetActive(false);
 

    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;

        player.Enable();
        UI.Disable();
        openInv = input.FindAction("Inventory");

        HUD.SetActive(true);
    }
}
