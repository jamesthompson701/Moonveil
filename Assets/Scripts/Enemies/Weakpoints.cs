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

    [Header("Gem Visual")]
    public Renderer[] gemRenderers;

    public Material fireMaterial;
    public Material waterMaterial;
    public Material airMaterial;
    public Material earthMaterial;

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
                SetGemMaterial(earthMaterial);
                break;
            case ElementType.Fire:
                SetGemMaterial(fireMaterial);
                break;
            case ElementType.Air:
                SetGemMaterial(airMaterial);
                break;
            case ElementType.Water:
                SetGemMaterial(waterMaterial);
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
            gameObject.SetActive(false);
        }
    }

    void SetGemMaterial(Material mat)
    {
        if (mat == null)
        {
            Debug.LogWarning($"{nameof(SetGemMaterial)} called with null material on '{name}'.");
            return;
        }

        if (gemRenderers == null || gemRenderers.Length == 0)
        {
            gemRenderers = GetComponentsInChildren<Renderer>();
            if (gemRenderers == null || gemRenderers.Length == 0)
            {
                Debug.LogWarning($"{nameof(Weakpoints)} on '{name}' found no Renderers to set.");
                return;
            }
        }

        foreach (Renderer r in gemRenderers)
        {
            if (r == null)
                continue;

            r.material = mat;
        }
    }
}