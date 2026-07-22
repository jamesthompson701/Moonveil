using UnityEngine;

public class ObjectBob : MonoBehaviour
{
    public float speed = 2f; // Speed of the bobbing motion

    [Tooltip("Use small number for subtle bobbing (Like under .1), larger for more pronounced effect.")]
    public float amplitude = 0.5f; // Amplitude of the bobbing motion

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Store the initial position of the object
    }

    private void Update()
    {
        // Calculate the new Y position using a sine wave for bobbing effect
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        // Update the object's position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}


