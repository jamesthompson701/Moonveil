using System.Collections;
using UnityEngine;

public class HUD : MonoBehaviour
{
    GameObject playerRef;
    SpellManager attackManagerRef;

    public GameObject[] highlight;

    

    private void Awake()
    {
        playerRef = GameObject.Find("Player");
        attackManagerRef = playerRef.GetComponent<SpellManager>();
         

    }
    private void Update()
    {
       switch (attackManagerRef.attackChoice)
        {
            case 1:
                SetActive(0);
                break;
            case 2:
                SetActive(1);
                break;
            case 3:
                SetActive(2);
                break;
            case 4:
                SetActive(3);
                break;
        }
    }

    public void SetActive(int index)
    {
        for (int i = 0; i < highlight.Length; i++)
        {
            highlight[i].SetActive(false);
        }
        highlight[index].SetActive(true);
    }
}
