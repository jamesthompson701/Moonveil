using UnityEngine;

/// <summary>
/// Stores external "impulses" so a CharacterController-based player can still be knocked back. We would have to completely remake movement otherwise.
/// Integrate by adding ConsumeDisplacement() into the single CharacterController.Move call per frame.
/// </summary>
public class PlayerImpactReceiver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;

    [Header("Impact Settings")]
    [Tooltip("How quickly the impact velocity decays back to zero (m/s^2).")]
    [SerializeField, Min(0f)] private float impactDamping = 18f;

    [Tooltip("Max magnitude of stored impact velocity (m/s).")]
    [SerializeField, Min(0f)] private float maxImpactSpeed = 12f;

    private Vector3 _impactVelocity;

    private void Awake()
    {
        if (!controller) controller = GetComponent<CharacterController>();
    }

    public void AddImpulse(Vector3 impulseVelocity)
    {
        _impactVelocity += impulseVelocity;

        if (_impactVelocity.magnitude > maxImpactSpeed)
            _impactVelocity = _impactVelocity.normalized * maxImpactSpeed;
    }

    public Vector3 ConsumeDisplacement(float deltaTime)
    {
        Vector3 displacement = _impactVelocity * Mathf.Max(0f, deltaTime);
        _impactVelocity = Vector3.MoveTowards(_impactVelocity, Vector3.zero, impactDamping * Mathf.Max(0f, deltaTime));
        return displacement;
    }
}

