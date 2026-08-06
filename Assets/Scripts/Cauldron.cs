using UnityEngine;

public class Cauldron : MonoBehaviour
{

    public GameObject fire;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireSpell"))
        {
            fire.SetActive(true);
            Invoke("DeactivateFlame", 60f);

            if (gameObject.CompareTag("Crafting"))
            {
                SpellManager2.Instance.inMenu = true;
                CanvasManager.Instance.OpenMenu(3);
            }
        }

    }

    public void DeactivateFlame()
    {
        fire.SetActive(false);
    }
}
