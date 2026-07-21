using UnityEngine;

/// <summary>
/// Weakpoint component.
/// - chooses an element at Start (random)
/// - can be activated / deactivated by an owner implementing IWeakpointOwner
/// - notifies owner when hit
/// </summary>
[RequireComponent(typeof(Collider))]
public class Weakpoints : MonoBehaviour
{
    public PengKingBoss pengKing;

    public GameObject fireWeakpoint;
    public GameObject waterWeakpoint;
    public GameObject airWeakpoint;
    public GameObject earthWeakpoint;

    public enum ElementType
    {
        Fire,
        Water,
        Earth,
        Air
    }

    public ElementType elementType;

    public void OnEnable()
    {
        int elementIndex = Random.Range(0, System.Enum.GetValues(typeof(ElementType)).Length);
        elementType = (ElementType)elementIndex;

        switch (elementType)
        {
            case ElementType.Earth:
                earthWeakpoint.SetActive(true);
                break;
            case ElementType.Fire:
                fireWeakpoint.SetActive(true);
                break;
            case ElementType.Air:
                airWeakpoint.SetActive(true);
                break;
            case ElementType.Water:
                waterWeakpoint.SetActive(true);
                break;
        }

    }

    // Handle normal physics collisions
    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    // Handle trigger collisions (projectiles or spells often use triggers)
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        if (other == null)
            return;

        // Defensive: ensure pengKing reference
        if (pengKing == null)
        {
            Debug.LogWarning($"{nameof(Weakpoints)} on '{name}' has no pengKing reference assigned.");
        }

        // CompareTag is faster and avoids exceptions if tag missing
        if (other.CompareTag("TillSpell") && elementType == ElementType.Earth ||
            other.CompareTag("FireSpell") && elementType == ElementType.Fire ||
            other.CompareTag("HarvestSpell") && elementType == ElementType.Air ||
            other.CompareTag("WateringSpell") && elementType == ElementType.Water)
        {
            pengKing?.WeakpointDestroyed();

            switch (elementType)
            {
                case ElementType.Earth:
                    earthWeakpoint.SetActive(false);
                    break;
                case ElementType.Fire:
                    fireWeakpoint.SetActive(false);
                    break;
                case ElementType.Air:
                    airWeakpoint.SetActive(false);
                    break;
                case ElementType.Water:
                    waterWeakpoint.SetActive(false);
                    break;
            }

            gameObject.SetActive(false);
        }
    }
}