using StarterAssets;
using Unity.Collections;
using UnityEngine;

public enum eFastTravel { home, mainTown, fireTown, waterTown, earthTown }
public class EnvironmentManager : MonoBehaviour
{
    [NamedArray(typeof(eFastTravel))] public GameObject[] fastTravelShrines;

    public static EnvironmentManager Instance;

    public GameObject player;
    public CharacterController characterController;
    //public ThirdPersonController thirdPersonController;

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
        Debug.Log("Teleport player to " +  destination + " at these cords: " + fastTravelShrines[(int)destination].transform.position);
        characterController.enabled = false;
        player.transform.position = fastTravelShrines[(int)destination].transform.Find("TP Point").position;
        characterController.enabled = true;

        if (destination == eFastTravel.home)
        {
            player.GetComponent<SpellManager2>().inCombatArea = false;
            AudioManager.ChangeTrack(eMusic.fireIslandDay);
        }

        if(destination == eFastTravel.fireTown)
        {
            player.GetComponent<SpellManager2>().inCombatArea = true;
            AudioManager.ChangeTrack(eMusic.mainIslandDay);
        }
    }

}
