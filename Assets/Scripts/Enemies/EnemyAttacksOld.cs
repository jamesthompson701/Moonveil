using UnityEngine;

public class EnemyAttacksOld : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool isMelee;
    private void OnCollisionEnter(Collision collision)
    {
        PlayerDamageReceiver player = collision.collider.GetComponent<PlayerDamageReceiver>();
    
        if (!isMelee)
        {
            if (collision.collider.CompareTag("Enemy")) return;
    
            if (player != null)
            {
                player.TakeDamage(damage);
            }
    
            if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
    
            else Destroy(gameObject, 5f);
        }
        else if (isMelee)
        {
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    PlayerManager player = other.GetComponent<PlayerManager>();
    //    if (!isMelee)
    //    {
    //        if (player != null)
    //        {
    //            player.TakeDamage(damage);
    //        }
    //        if (other.CompareTag("Ground") || other.CompareTag("Player"))
    //        {
    //            Destroy(gameObject);
    //        }
    //        else Destroy(gameObject, 5f);
    //    }
    //    else if (isMelee)
    //    {
    //        if (player != null)
    //        {
    //            player.TakeDamage(damage);
    //        }
    //    }
    //}
}
