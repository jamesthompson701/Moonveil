using UnityEngine;

public class SelfDestructFunction : MonoBehaviour
{

    public void SelfDestruct()
    {
        // I WANNA MAKE MY MURDER LOOK LIKE A SUICIDE
        Destroy(this.gameObject);
    }
}
