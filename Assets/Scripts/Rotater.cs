using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float spinSpeed = 20f;
    public bool isVertical = true;
    void Update()
    {
        if (isVertical)
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        else 
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }
}
