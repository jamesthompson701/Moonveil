using UnityEngine;

public class FishingTargetController : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0, z);

        transform.position += move * speed * Time.deltaTime;
    }
}