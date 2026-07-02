using UnityEngine;

public class FishingBubble : MonoBehaviour
{
    // movement
    public float radius = 5f;
    public float moveSpeed = 3f;

    public BoxCollider movementBounds;

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

        Debug.DrawLine(transform.position,targetPos,Color.red);
    }

    public void BeginBubblePhase()
    {
        if(movementBounds != null)
        {
            transform.position = movementBounds.bounds.center;
        }

        PickNewTarget();
    }

    // detection
    void PickNewTarget()
    {
        if(movementBounds == null)
        {
            Debug.LogError("Movement Bounds Missing!");
            return;
        }

        Bounds bounds = movementBounds.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        targetPos = new Vector3(x, y, bounds.center.z);
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