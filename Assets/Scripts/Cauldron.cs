using UnityEngine;

public class Cauldron : MonoBehaviour
{

    public GameObject fire;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireSpell"))
        {
            fire.SetActive(true);
        }
    }
}
