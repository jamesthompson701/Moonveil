using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;

public class DebugCanvas : MonoBehaviour
{
    public GameObject player;
    public Slider movementSlider;

    public DatabaseSO database;
    public TMP_Dropdown dropdown;

    public static DebugCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

        private void Start()
    {
        movementSlider.value = 4.0f;
    }

    public void MovementSpeed()
    {
        ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
        playerController.MoveSpeed = movementSlider.value;
    }

    public void GiveItem()
    {
        foreach (var item in database.items)
        {
            
        }
    }


}
