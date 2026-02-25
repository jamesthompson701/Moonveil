using UnityEngine;

public class CropFire : MonoBehaviour
{
    //dissipates after a couple seconds
    private void Awake()
    {
        Destroy(gameObject, 0.66f);
    }
    private void Update()
    {
        gameObject.transform.localScale = gameObject.transform.localScale - new Vector3(0.008f, 0.008f, 0.008f);
    }
}
