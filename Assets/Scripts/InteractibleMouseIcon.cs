using UnityEngine;

public class InteractibleMouseIcon : MonoBehaviour
{
    public GameObject rightClickIcon;
    public GameObject weenie;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rightClickIcon.SetActive(true);
            if (weenie != null)
            {
                Destroy(weenie);
            }
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
