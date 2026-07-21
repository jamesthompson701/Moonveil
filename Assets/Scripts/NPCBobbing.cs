using UnityEngine;

public class NPCBobbing : MonoBehaviour
{
    public float bobMin;
    public float bobMax;

    private bool bobToggle;

    public Transform myTransform;

    void Update()
    {
        if (myTransform.localScale.y < bobMax)
        {
            if(bobToggle)
            {
                //myTransform.localScale = new Vector
            }
        }
    }
}
