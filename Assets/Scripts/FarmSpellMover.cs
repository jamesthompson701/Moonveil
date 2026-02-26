using UnityEngine;

public class FarmSpellMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float travelDistance;
    private float lifetime;

    private Vector3 startPos;
    private float aliveTime;

    public void Init(Vector3 dir, float spd, float travelDist, float life, eEffects soundEffect)
    {
        direction = dir.normalized;
        speed = spd;
        travelDistance = Mathf.Max(0.01f, travelDist);
        lifetime = Mathf.Max(0.01f, life);

        startPos = transform.position;
        aliveTime = 0f;

        AudioManager.PlayOneShot(soundEffect, transform, 100);

        Debug.Log("playing sound" + soundEffect);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        aliveTime += dt;

        transform.position += direction * (speed * dt);

        float moved = Vector3.Distance(startPos, transform.position);

        if (moved >= travelDistance || aliveTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
