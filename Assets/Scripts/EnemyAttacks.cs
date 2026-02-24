using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerManager player = collision.collider.GetComponent<PlayerManager>();
        if (collision.collider.CompareTag("Enemy")) return;
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
