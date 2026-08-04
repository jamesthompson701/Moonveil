using UnityEngine;

public class MiningPushZone : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 175f;

    [Header("Behavior")]
    [SerializeField] private bool instantPush = false;

    private bool hasPushed;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (instantPush)
        {
            PushPlayer(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (instantPush)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PushPlayer(other);
    }

    private void PushPlayer(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller == null)
        {
            return;
        }

        if (instantPush && hasPushed)
        {
            return;
        }

        Vector3 pushDirection = transform.parent.up;

        controller.Move(pushDirection * pushForce * Time.deltaTime);

        if (instantPush)
        {
            hasPushed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPushed = false;
        }
    }
}