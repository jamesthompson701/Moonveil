using UnityEngine;

/// <summary>
/// Handles earth attack hitbox logic: applies damage, knockup, and stun.
/// </summary>
public class EarthAttackHitbox : MonoBehaviour
{
    [Header("Earth Attack Settings")]
    [Tooltip("Damage dealt to enemies.")]
    public int damage;
    [Tooltip("Knockup force applied.")]
    public float knockupForce;
    [Tooltip("Stun duration in seconds.")]
    public float stunDuration;
    [Tooltip("Reference to the caster.")]
    public GameObject caster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            CreatureDefs enemy = other.GetComponentInParent<CreatureDefs>();
            if (enemy != null)
            {
                // Apply damage and knockup
                //enemy.TakeDamage(damage, other.ClosestPoint(transform.position), Vector3.up, knockupForce, caster);
                // Apply stun (prevent movement/attack)
                enemy.StartCoroutine(StunCoroutine(enemy, stunDuration));
            }
        }
    }

    private System.Collections.IEnumerator StunCoroutine(CreatureDefs enemy, float duration)
    {
        Debug.Log("we are stunning you for " + duration);
        enemy.enabled = false;
        yield return new WaitForSeconds(duration);
        enemy.enabled = true;
    }
}
