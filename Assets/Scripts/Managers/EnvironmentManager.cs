using StarterAssets;
using Unity.Collections;
using UnityEngine;

public enum eFastTravel { home, mainTown, fireTown, waterTown, earthTown }
public class EnvironmentManager : MonoBehaviour
{
    [NamedArray(typeof(eFastTravel))] public GameObject[] fastTravelShrines;

    public static EnvironmentManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Travel(eFastTravel destination)
    {
        Transform playerTransform = ThirdPersonController.Instance.transform;
        playerTransform = fastTravelShrines[(int)destination].transform;
    }

}
