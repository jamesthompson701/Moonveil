using UnityEngine;

public class MiningPushZone : MonoBehaviour
{
    [SerializeField] private float pushForce = 175;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller == null)
        {
            return;
        }

        Vector3 pushDirection = transform.parent.up;

        controller.Move(pushDirection * pushForce * Time.deltaTime);
    }
}