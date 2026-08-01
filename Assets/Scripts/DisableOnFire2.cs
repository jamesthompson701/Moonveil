using UnityEngine;

public class DisableOnFire2 : MonoBehaviour
{
    private Renderer ArenaRen;
    private Collider ArenaCol;

    private void Awake()
    {
        ArenaRen = GetComponent<Renderer>();
        ArenaCol = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire2"))
        {
            DisableArena();
        }
    }

    public void DisableArena()
    {
            ArenaRen.enabled = false;
            ArenaCol.enabled = false;
            enabled = false;
    }
}
