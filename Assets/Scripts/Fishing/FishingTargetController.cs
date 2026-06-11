using UnityEngine;

public class FishingTargetController : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x,0,z);

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
