using UnityEngine;

public class InteractibleMouseIcon : MonoBehaviour
{
    public GameObject rightClickIcon;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("ItemPickupTag"))
        {
            rightClickIcon.SetActive(true);
        }
        else
        {
            rightClickIcon.SetActive(false);
        }
    }
}
