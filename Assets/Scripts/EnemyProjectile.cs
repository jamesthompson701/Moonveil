using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;

    private Vector3 _start;               
    private Vector3 _target;              
    private bool _useArc;                 
    private float _arcHeight;             
    private float _t;                     
    private float _distance;              
    private bool _initialized;            

    // called from EnemyAI after instantiation
    public void Init(Vector3 targetPosition, float projectileSpeed, bool useArc, float arcHeight)
    {
        _start = transform.position;
        _target = targetPosition;
        speed = projectileSpeed;

        _useArc = useArc;
        _arcHeight = arcHeight;

        _distance = Vector3.Distance(_start, _target);
        _t = 0f;
        _initialized = true;

        // Face the general direction for straight shots
        transform.LookAt(_target);
    }

    private void Update()
    {
        if (!_initialized) return;

        if (_distance <= 0.01f)
        {
            Destroy(gameObject);
            return;
        }

        // Move parameter based on speed
        _t += (Time.deltaTime * speed) / _distance;

        if (_t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = Vector3.Lerp(_start, _target, _t);

        if (_useArc)
        {
            // Sin curve peaks at t=0.5 and returns to 0 at t=1
            float height = Mathf.Sin(_t * Mathf.PI) * _arcHeight;
            pos += Vector3.up * height;
        }

        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
