using UnityEngine;

/// <summary>
/// Expands the X and Y scale of an object to a chosen max over a duration without adjusting the Z scale.
/// </summary>
public class SizeIncreaser : MonoBehaviour
{
    [Tooltip("Target X scale to reach.")]
    [SerializeField] private float maxX = 2f;

    [Tooltip("Target Y scale to reach.")]
    [SerializeField] private float maxY = 2f;

    [Tooltip("Duration in seconds to reach the target scale.")]
    [SerializeField] private float duration = 1f;

    private Vector3 _originalScale;
    private float _elapsedTime;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _elapsedTime = 0f;
    }

    private void Update()
    {
        if (_elapsedTime < duration)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / duration);

            float newX = Mathf.Lerp(_originalScale.x, maxX, t);
            float newY = Mathf.Lerp(_originalScale.y, maxY, t);

            transform.localScale = new Vector3(newX, newY, _originalScale.z);
        }
    }
}
