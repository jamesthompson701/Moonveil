using UnityEngine;

public class Rotater : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 20 * Time.deltaTime);
    }
}
