using UnityEngine;

public class SunFollowsPlayer : MonoBehaviour
{
    //maeve is the center of the universe
    public GameObject player;

    void Update()
    {
        this.gameObject.transform.position = player.transform.position;
    }
}
