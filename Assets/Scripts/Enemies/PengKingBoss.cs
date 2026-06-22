using System.Collections.Generic;
using UnityEngine;

public class PengKingBoss : MonoBehaviour
{
    public Collider shieldCollider;
    public Renderer shieldRenderer;
    public Collider arenaCollider;
    public Renderer arenaRenderer;

    public CreatureDefs pengKing;
    public int destroyedWeakpointsCount = 0;
    public int activeWeakpointsCount = 0;
    private bool isBossDead = false;
    private bool fightStarted = false;

    public GameObject[] weakpoints; // Array to hold weakpoint GameObjects

    public void Awake()
    {
        DeactivateShield();
    }

    public void Update()
    {
        CheckBossHealth();
    }

    public void CheckBossHealth()
    {
        float currentHealth = pengKing.HealthPercent;
        switch (currentHealth)
        {
            case <= 0.75f when activeWeakpointsCount == 0:
                ActivateShieldAndRandomWeakpoints();
                break;
            case <= 0.5f when activeWeakpointsCount == 0:
                ActivateShieldAndRandomWeakpoints();
                break;
            case <= 0.25f when activeWeakpointsCount == 0:
                ActivateShieldAndRandomWeakpoints();
                break;
        }
        if (currentHealth <= 0)
        {
            isBossDead = true;
            // Boss is defeated, handle defeat logic here
            Debug.Log("Peng King Boss defeated!");
            DeactivateShield();
            // You can add additional logic such as playing a death animation, dropping loot, etc.
        }
    }

    public void WeakpointDestroyed()
    {
        destroyedWeakpointsCount++;
        activeWeakpointsCount = Mathf.Max(0, activeWeakpointsCount - 1);
        if (destroyedWeakpointsCount >= 3)
        {
            // All weakpoints destroyed, deactivate shield
            DeactivateShield();
        }
    }

    public void DeactivateShield()
    {
        if (shieldCollider != null)
            shieldCollider.enabled = false;
        if (shieldRenderer != null)
            shieldRenderer.enabled = false;

        if (isBossDead || !fightStarted)
        {
            if (arenaCollider != null)
                arenaCollider.enabled = false;
            if (arenaRenderer != null)
                arenaRenderer.enabled = false;
            fightStarted = true;
        }
    }


    public void ActivateShieldAndRandomWeakpoints()
    {
        if (shieldCollider != null)
            shieldCollider.enabled = true;
        if (shieldRenderer != null)
            shieldRenderer.enabled = true;
        if (arenaCollider != null)
            arenaCollider.enabled = true;
        if (arenaRenderer != null)
            arenaRenderer.enabled = true;

        if (weakpoints == null || weakpoints.Length == 0 || activeWeakpointsCount > 0)
            return;

        // Deactivate all weakpoints first
        for (int i = 0; i < weakpoints.Length; i++)
        {
            if (weakpoints[i] != null)
                weakpoints[i].SetActive(false);
        }

        int toActivate = Mathf.Min(3, weakpoints.Length);
        var chosenIndices = new HashSet<int>();

        // Pick `toActivate` distinct random indices
        while (chosenIndices.Count < toActivate)
        {
            int idx = Random.Range(0, weakpoints.Length);
            // Ensure the weakpoint exists (defensive)
            if (weakpoints[idx] != null)
                chosenIndices.Add(idx);
        }

        // Activate chosen weakpoints
        foreach (int idx in chosenIndices)
        {
            weakpoints[idx].SetActive(true);
        }

        // Reset destroyed count for the new wave and set active count
        destroyedWeakpointsCount = 0;
        activeWeakpointsCount = toActivate;
    }
}
