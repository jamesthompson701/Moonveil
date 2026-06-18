using UnityEngine;

public class FishingBubble : MonoBehaviour
{
    // movement
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
        //movement
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,targetPos) < .2f)
        {
            PickNewTarget();
        }

        //detection
                Vector3 view = Camera.main.WorldToViewportPoint(bubble.position);

        bool visible = view.z > 0 && view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1;

        if(visible)
        {
            visabliityTimer = 0;
        }
        else
        {
            visabliityTimer += Time.deltaTime;

            if(visabliityTimer >= 3f)
            {
                FishingManager.Instance.FailFishing();
            }
        }
    }

    // detection
    void PickNewTarget()
    {
        Vector2 random =Random.insideUnitCircle * radius;

        targetPos = startPos + new Vector3(random.x,0,random.y);
    }

    public float failTime = 2f;

    float timer;

    private void OnTriggerStay(Collider other)
    {
        ElementZone zone = other.GetComponent<ElementZone>();

        if(zone == null)
        {
            return;
        }

        bool correct = CheckPlayerElement(zone.requiredElement);

        if(correct)
        {
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;

            if(timer >= failTime)
            {
                FishingManager.Instance.FailFishing();
            }
        }
    }

    bool CheckPlayerElement(MineralType required)
    {
        return true;
    }

    // visability checking
    public Transform bubble;

    float visabliityTimer;
}