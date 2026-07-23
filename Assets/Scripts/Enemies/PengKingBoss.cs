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
    public bool fightStarted = false;

    public GameObject[] weakpoints; // Array to hold weakpoint GameObjects

    // --- Spawn tracking for minions created by the boss ---
    [Tooltip("Number of active spawned minions created by the boss.")]
    public int spawnedMinionsCount = 0;
    [Tooltip("Maximum allowed spawned minions before the boss stops attacking.")]
    public int maxSpawnedMinions = 5;

    public PlayerDamageReceiver playerDamageReceiver;

    private readonly float[] healthThresholds = new float[] { 0.75f, 0.5f, 0.25f };
    private int _nextThresholdIndex = 0;

    public void Awake()
    {
        DeactivateShield();
    }

    public void Update()
    {
        CheckBossHealth();
        CheckForPlayerDeath();
    }

    public void CheckBossHealth()
    {
        if (pengKing == null) return;

        float currentHealth = pengKing.HealthPercent;

        // Trigger the next threshold in sequence, only if there are no active weakpoints
        if (_nextThresholdIndex < healthThresholds.Length &&
            currentHealth <= healthThresholds[_nextThresholdIndex] &&
            activeWeakpointsCount == 0)
        {
            float threshold = healthThresholds[_nextThresholdIndex];
            ActivateShieldAndRandomWeakpoints(threshold);
            _nextThresholdIndex++;
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

        // Remove invulnerability when the shield goes down
        if (pengKing != null)
        {
            pengKing.ClearHealthFloorPercent();
            // Make sure the boss can attack again when shield is down
            pengKing.SetCanAttack(true, abortCurrentAttack: false);
        }

        // Keep phase state consistent
        destroyedWeakpointsCount = 0;
        activeWeakpointsCount = 0;

        if (isBossDead || !fightStarted)
        {
            if (arenaCollider != null)
                arenaCollider.enabled = false;
            if (arenaRenderer != null)
                arenaRenderer.enabled = false;
            fightStarted = false;
        }
    }


    public void ActivateShieldAndRandomWeakpoints(float floorThreshold = 0f)
    {
        fightStarted = true;

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

        // Set the boss health floor so it cannot drop below this threshold while wave is active
        if (pengKing != null && floorThreshold > 0f)
        {
            pengKing.SetHealthFloorPercent(floorThreshold);
        }
    }

    // --- API for spawn tracking ---
    // Call when the boss's projectile/prefab spawns a minion.
    public void RegisterSpawnedMinion()
    {
        spawnedMinionsCount++;
        UpdateAttackState();
    }

    // Call when a spawned minion dies / is destroyed.
    public void UnregisterSpawnedMinion()
    {
        spawnedMinionsCount = Mathf.Max(0, spawnedMinionsCount - 1);
        UpdateAttackState();
    }

    private void UpdateAttackState()
    {
        if (pengKing != null)
        {
            bool allowAttack = spawnedMinionsCount == 0;
            // Do not abort current attack by default; set second arg true if you want immediate abort.
            pengKing.SetCanAttack(allowAttack, abortCurrentAttack: false);
        }
    }

    public void DeactivateArena()
    {
        if (arenaCollider != null)
            arenaCollider.enabled = false;
        if (arenaRenderer != null)
            arenaRenderer.enabled = false;
    }

    private void CheckForPlayerDeath()
    {
        if (playerDamageReceiver != null)
        {
            if (playerDamageReceiver.currentHealth <= 0f)
            {
                // Player has died, handle logic here
                Debug.Log("Player has died during the boss fight!");

                DeactivateShield();
                DeactivateArena();

                pengKing.ResetHealth();

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    if (weakpoints[i] != null)
                        weakpoints[i].SetActive(false);
                }
            }
        }
    }
}