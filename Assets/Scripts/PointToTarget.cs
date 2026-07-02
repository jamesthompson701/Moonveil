using UnityEngine;

public class PointToTarget : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.LookAt(target);
    }

    public void ChangeTarget(Transform _newTarget)
    {
        target = _newTarget;
    }
}
