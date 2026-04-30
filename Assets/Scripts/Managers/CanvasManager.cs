using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

[DefaultExecutionOrder(-100)] //change for mining fade canvas staying on enter/exit bug
public class CanvasManager : MonoBehaviour
{

    public GameObject miniGameCanvas;

    // TRACKS CURRENT ACTIVE CANVAS; 0 = HUD/NONE ACTIVE ; 999 = MINIGAMES
    int currentCanvas = 0;
    bool miniGame = false;

    [Header("DO NOT MOVE THINGS you can add though")]
    [SerializeField] private GameObject[] menus;
    // 0 - HUD; 1 - Book; 2 - SelectionWheel; 3 - Workbench ; 4 - Inventory; 5 - FastTravel



    //In awake this was initialize with all the canvases that we want to close with esc
    [SerializeField] private GameObject[] escCloseableCanvases;


    // GETS THE KEYBINDS
    public InputActionAsset input;
    InputAction inventoryAction;
    InputAction pauseAction;
    InputAction selectionAction;

    // CONTROLS WHAT KEYBINDS DO
    InputActionMap playerMap;
    InputActionMap UIMap;

    public static CanvasManager Instance;


    private void Awake()
    {
        //When adding a new menu you want to close with esc add it to this array
        escCloseableCanvases = new GameObject[] { menus[5], menus[3], miniGameCanvas};

        inventoryAction = input.FindAction("Inventory");
        pauseAction = input.FindAction("Pause");
        selectionAction = input.FindAction("Selection");

        playerMap = input.FindActionMap("Player");
        UIMap = input.FindActionMap("UI");


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

    // Needed for cursor to lock into game
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClickSelector.Instance.enabled = true;
        }
    }

    private void Update()
    {
        // THIS IS JUST FOR DEBUG MENU
        if (Input.GetKey(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ClickSelector.Instance.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClickSelector.Instance.enabled = true;
        }

        bool inv = inventoryAction.WasPressedThisFrame();
        if (inv)
        {
            OpenMenu(4);
           
        }

        // Opens pause menu and closes whatever is open
        bool pause = this.pauseAction.WasPressedThisFrame();
        
        if (pause)
        {
            if (currentCanvas != 0)
            {
                //Closes current canvas
                OpenMenu(currentCanvas);
            }
            else
            {
                // Checks if mini game canvases are open
                if (currentCanvas != 999)
                {
                    //Opens pause menu
                    OpenMenu(1);
                }

            }
                 
        }

        bool selection = selectionAction.WasPressedThisFrame();
        if (selection)
        {
            OpenMenu(2);
        }
    }



    public void OpenMenu(int menu)
    {
        if (!menus[menu].activeInHierarchy)
        {
            CloseAllMenus();

            menus[menu].SetActive(true);
            currentCanvas = menu;

            playerMap.Disable();
            UIMap.Enable();
            menus[0].GetComponent<Canvas>().enabled = false;

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ClickSelector.Instance.enabled = false;
        }
        else if (menus[menu].activeInHierarchy)
        {
            CloseAllMenus();
        }

        //Resets all actions
        inventoryAction = input.FindAction("Inventory");
        pauseAction = input.FindAction("Pause");
        selectionAction = input.FindAction("Selection");

    }

    public void CloseAllMenus()
    {
        for (int i = 1; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
            currentCanvas = 0;

            playerMap.Enable();
            UIMap.Disable();

            menus[0].GetComponent<Canvas>().enabled = true;

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClickSelector.Instance.enabled = true;
        }
    }

    public void OpenMiniGame(GameObject canvas)
    {
        canvas.SetActive(true);
        currentCanvas = 999;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;

    }

    public void CloseMiniGame(GameObject canvas)
    {
        canvas.SetActive(false);
        miniGame = true;
        currentCanvas = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;
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


