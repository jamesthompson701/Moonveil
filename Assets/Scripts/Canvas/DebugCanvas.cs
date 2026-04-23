using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    public GameObject player;
    public Slider movementSlider;

    private void Start()
    {
        movementSlider.value = 4.0f;
    }

    public void MovementSpeed()
    {
        ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
        playerController.MoveSpeed = movementSlider.value;
    }
}
