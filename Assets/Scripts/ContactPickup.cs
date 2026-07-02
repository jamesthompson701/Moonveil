using UnityEngine;

public class ContactPickup : MonoBehaviour
{
    //destroys itself and gives the player an item when maeve touches it
    public ItemSO item;

    private void Start()
    {
        Invoke("SelfDestruct", 60f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.instance.invSO.AddItem(item, 1);
            Destroy(this.gameObject);
            //this.gameObject.SetActive(false);
        }
    }

    private void SelfDestruct()
    {
        Destroy(this.gameObject);
    }
}
 
