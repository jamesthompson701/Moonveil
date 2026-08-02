using UnityEngine;

public class InteractibleMouseIcon : MonoBehaviour
{
    public GameObject rightClickIcon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rightClickIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rightClickIcon.SetActive(false);
        }
    }
}
