using UnityEngine;

public class Cauldron : MonoBehaviour
{

    public GameObject fire;
    private void OnTriggerEnter(Collider other)
    {
        //if it's a water spell, soil becomes wet
        if (other.CompareTag("FireSpell"))
        {
            fire.SetActive(true);
        }
    }
}
