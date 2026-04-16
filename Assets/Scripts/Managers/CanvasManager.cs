using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CanvasManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public GameObject menuCanvas;
    public GameObject fastTravelCanvas;
    public GameObject selectionCanvas;
    public GameObject HUD;
    public GameObject workbenchCanvas;
    public GameObject titleScreenCanvas;
    public GameObject miniGameCanvas;
    bool isActive = false;

    //In awake this was initialize with all the canvases that we want to close with esc
    [SerializeField] private GameObject[] escCloseableCanvases;

    public InputActionAsset input;
    InputAction openInv;
    InputAction openPause;
    InputAction openSelection;

    InputActionMap player;
    InputActionMap UI;

    public static CanvasManager Instance;


    private void Awake()
    {
        //When adding a new menu you want to close with esc add it to this array
        //keira note: use this only for canvases you can't open with a keypress
        //   - add ESC to the UI action in input actions in project settings for keypress menus
        // another note this doesnt work for selection canvas it is wip
        escCloseableCanvases = new GameObject[]{fastTravelCanvas, workbenchCanvas, miniGameCanvas};

        openInv = input.FindAction("Inventory");
        openPause = input.FindAction("Pause");
        openSelection = input.FindAction("Selection");

        player = input.FindActionMap("Player");
        UI = input.FindActionMap("UI");


        //Making canvas manager a singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New Canvas Manager");
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
            Debug.Log("Esc Pressed");
            bool canvasClosed = false;
            foreach (GameObject canvas in escCloseableCanvases)
            {
                Debug.Log("Canvas " +  canvas.name + " is " + canvas.activeInHierarchy);
                if(canvas.activeInHierarchy)
                {
                    
                    if(canvas == miniGameCanvas)
                    {
                        if (FishingManager.Instance.inFishingMode)
                        {
                            Debug.Log("In fishing mode so we exit");
                            canvasClosed = true;
                            CloseMiniGame();
                        }
                        
                    }
                    else
                    {
                        canvasClosed = true;
                        isActive = false;
                        canvas.SetActive(false);
                        CloseMenu();
                    }
                    
                }
            }
            if (!canvasClosed)
            {
                OpenPause();
            }
        }

        bool selection = openSelection.WasPressedThisFrame();
        if (selection == true)
        {
            OpenSelectionWheel();
        }


    }
    /*
     *         bool esc = escMenu.WasPressedThisFrame();
        if (esc == true)
        {
            if (!menuCanvas.activeInHierarchy)
            {
                //count the inactive menus
                int inactiveMenus = 0;
                foreach (GameObject _menu in escCloseableMenus)
                {
                    if (!_menu.activeInHierarchy)
                    {
                        inactiveMenus++;
                        Debug.Log("inactive menus: " + inactiveMenus);
                        Debug.Log("number of closeable menus: " + escCloseableMenus.Count);
                    }

                }

                //if all the menus were inactive, open the pause menu
                if (inactiveMenus == allMenus.Count)
                {
                    OpenPause();
                }
                //otherwise, close all the menus closeable by esc
                else
                {
                    foreach (GameObject _menu in escCloseableMenus)
                    {
                        if (_menu.activeInHierarchy)
                        {
                            isActive = false;
                            _menu.SetActive(false);
                            CloseMenu();
                        }
                    }
                }
            }
            //or if the pause menu was active, just close it
            else
            {
                OpenPause();
            }
            
        }
     */

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
        openPause = input.FindAction("Pause");
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
        openPause = input.FindAction("Pause");
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

    public void OpenMiniGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;

        isActive = true;

        player.Disable();
        UI.Enable();

        HUD.GetComponent<Canvas>().enabled = false;
    }

    public void CloseMiniGame()
    {

        Debug.Log("Minegame Closed");
        FishingManager.Instance.ExitFishingMode();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;

        isActive = false;

        player.Enable();
        UI.Disable();
        openInv = input.FindAction("Inventory");

        HUD.GetComponent<Canvas>().enabled = true;
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;

        player.Disable();
        UI.Enable();

        HUD.GetComponent<Canvas>().enabled = false;

        Time.timeScale = 0f;

    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;

        player.Enable();
        UI.Disable();
        openInv = input.FindAction("Inventory");

        HUD.GetComponent<Canvas>().enabled = true;

        Time.timeScale = 1f;
    }

    //public void OpenTitleScreen()
    //{
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //    ClickSelector.Instance.enabled = false;

    //    player.Disable();
    //    UI.Enable();
    //}

    //public void CloseTitleScreen()
    //{
    //    CloseMenu();

    //    titleScreenCanvas.SetActive(false);

    //}


    //// handles taking screenshot
    //IEnumerator TakeScreenshot()
    //{
    //    yield return new WaitForEndOfFrame();

    //    ScreenCapture.CaptureScreenshotAsTexture();


    //}
}


