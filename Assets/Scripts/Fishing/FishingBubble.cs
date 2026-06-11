using UnityEngine;

public class FishingBubble : MonoBehaviour
{
    public float radius = 5f;
    public float moveSpeed = 3f;

    Vector3 targetPos;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        PickNewTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,targetPos) < .2f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        Vector2 random =Random.insideUnitCircle * radius;

        targetPos = startPos + new Vector3(random.x,0,random.y);
    }
}
