using UnityEngine;

public enum RockState
{
    Fresh, Cracked, Shattered
}

public enum MineralType
{
    Fire,
    Water,
    Earth
}

public class MineRock : MonoBehaviour
{
    public Transform cameraAnchor;
    public GameObject glowObject;

    public RockState state = RockState.Fresh;
    public MineralType mineralType;

    private MiningManager miningManager;

    void Start()
    {
        miningManager = FindFirstObjectByType<MiningManager>();
        UpdateVisuals();
    }

    public void Interact()
    {
        Debug.Log("Interact called on " + name);

        if (state == RockState.Shattered) return;

        miningManager.StartMining(this);
    }

    public void Fail()
    {
        if (state == RockState.Fresh)
            state = RockState.Cracked;
        else if (state == RockState.Cracked)
            state = RockState.Shattered;

        UpdateVisuals();
    }

    public void ResetRock()
    {
        state = RockState.Fresh;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (glowObject == null) return;

        glowObject.SetActive(state != RockState.Shattered);

        if (state == RockState.Cracked)
        {
            glowObject.transform.localScale = Vector3.one * 0.5f;
        }
        else
        {
            glowObject.transform.localScale = Vector3.one;
        }
    }
}