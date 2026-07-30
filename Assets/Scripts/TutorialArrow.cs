using UnityEngine;

public class TutorialArrow : MonoBehaviour
{

    public static TutorialArrow instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void PointAt(Transform _target)
    {
        this.gameObject.GetComponent<PointToTarget>().ChangeTarget(_target);
    }
}
