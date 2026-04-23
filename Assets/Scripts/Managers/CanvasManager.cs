using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

public class CanvasManager : MonoBehaviour
{

    public GameObject miniGameCanvas;

    // TRACKS CURRENT ACTIVE CANVAS; 0 = HUD/NONE ACTIVE
    int currentCanvas = 0;


    [Header("DO NOT MOVE THINGS you can add though")]
    [SerializeField] private GameObject[] menus;

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
        // THIS IS JUST FOR DEBUG
        if (Input.GetKey(KeyCode.BackQuote))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ClickSelector.Instance.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClickSelector.Instance.enabled = true;
        }

            bool inv = inventoryAction.WasPressedThisFrame();
        if (inv)
        {
            testOpenMenu(4);
           
        }

        bool pause = this.pauseAction.WasPressedThisFrame();
        if (pause)
        {
            if (currentCanvas != 0)
            {
                //Closes current canvas
                testOpenMenu(currentCanvas);
            }
            else
            {
                //Opens pause menu
                testOpenMenu(1);
            }

            //Debug.Log("Esc Pressed");
            //foreach (GameObject canvas in escCloseableCanvases)
            //{
            //    Debug.Log("Canvas " + canvas.name + " is " + canvas.activeInHierarchy);
            //    if (canvas.activeInHierarchy)
            //    {
            //        if (canvas == miniGameCanvas)
            //        {
            //            if (FishingManager.Instance.inFishingMode)
            //            {
            //                Debug.Log("In fishing mode so we exit");         
            //                CloseMiniGame();
            //            }

            //        }
            //    }
            //}

        }

        bool selection = selectionAction.WasPressedThisFrame();
        if (selection)
        {
            testOpenMenu(2);
        }
    }



    public void testOpenMenu(int menu)
    {
        if (!menus[menu].activeInHierarchy)
        {
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
            menus[menu].SetActive(false);
            currentCanvas = 1;

            playerMap.Enable();
            UIMap.Disable();

            menus[0].GetComponent<Canvas>().enabled = true;

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClickSelector.Instance.enabled = true;
        }

        //Resets all actions
        inventoryAction = input.FindAction("Inventory");
        pauseAction = input.FindAction("Pause");
        selectionAction = input.FindAction("Selection");

    }

    public void OpenMiniGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClickSelector.Instance.enabled = false;

        playerMap.Disable();
        UIMap.Enable();

    }

    public void CloseMiniGame()
    {

        Debug.Log("Minegame Closed");
        FishingManager.Instance.ExitFishingMode();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClickSelector.Instance.enabled = true;


        playerMap.Enable();
        UIMap.Disable();
        inventoryAction = input.FindAction("Inventory");

 
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


