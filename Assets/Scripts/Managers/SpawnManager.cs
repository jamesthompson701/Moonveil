using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [Tooltip("Enemy prefabs to spawn. Each entry is mapped index-wise to spawnPoints.")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Tooltip("Transforms where enemies will spawn. Each entry is mapped index-wise to enemyPrefabs.")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("If true, spawn/respawn when a new day begins (recommended). If false, use SpawnTime instead.")]
    public bool spawnOnNewDay = true;

    // current spawned instances; parallel to spawnPoints (null when empty or destroyed)
    private List<GameObject> spawnedInstances = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        // make sure lists are in a sane state
        EnsureSpawnedInstancesListSize();
    }

    private void Start()
    {
        // optionally spawn initial wave at start (comment this out if you don't want immediate spawn)
        SpawnAllMissing();
    }

    private void Update()
    {
        if (TimeManager.instance == null) return;

        float currentCycle = TimeManager.instance.dayLength;

        // Detect new day (cycle rollover) OR crossing the configured spawnTime
        bool shouldTrigger = false;

        if (spawnOnNewDay)
        {
            // DaylightCycleTime is reset to a small value when the day rolls over.
            // If lastObservedCycleTime > currentCycle we had a reset -> new day.
            if (currentCycle == 0)
            {
                shouldTrigger = true;
            }
        }

        if (shouldTrigger)
        {
            SpawnAllMissing();
        }
    }

    // Ensures spawnedInstances has the same capacity as spawnPoints (keeps indices aligned)
    private void EnsureSpawnedInstancesListSize()
    {
        int targetSize = Mathf.Max(0, spawnPoints != null ? spawnPoints.Count : 0);
        while (spawnedInstances.Count < targetSize) spawnedInstances.Add(null);
        while (spawnedInstances.Count > targetSize) spawnedInstances.RemoveAt(spawnedInstances.Count - 1);
    }

    // Spawns missing enemies for every configured spawn point (instantiates corresponding prefab by index)
    public void SpawnAllMissing()
    {
        EnsureSpawnedInstancesListSize();

        int maxIndex = Mathf.Min(enemyPrefabs != null ? enemyPrefabs.Count : 0, spawnPoints != null ? spawnPoints.Count : 0);

        for (int i = 0; i < maxIndex; i++)
        {
            if (spawnPoints[i] == null) continue; // skip invalid spawn point

            // If there's no instance or instance was destroyed, spawn/respawn
            if (spawnedInstances[i] == null)
            {
                SpawnAt(i);
            }
            else
            {
                // instance exists; optionally ensure it's active
                if (!spawnedInstances[i].activeInHierarchy)
                {
                    spawnedInstances[i].SetActive(true);
                }
            }
        }
    }

    // Instantiate prefab at spawnPoints[index] and store reference
    private void SpawnAt(int index)
    {
        if (index < 0 || index >= enemyPrefabs.Count || index >= spawnPoints.Count) return;
        var prefab = enemyPrefabs[index];
        var spawnPoint = spawnPoints[index];
        if (prefab == null || spawnPoint == null) return;

        GameObject inst = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        spawnedInstances[index] = inst;
    }

    // Optional API: let other systems notify manager that an enemy died (keeps list up-to-date immediately)
    // This is robust even if enemies are Destroyed without calling this method (manager will detect nulls on spawn time)
    public void RegisterEnemyDeath(GameObject enemy)
    {
        if (enemy == null) return;

        for (int i = 0; i < spawnedInstances.Count; i++)
        {
            if (spawnedInstances[i] == enemy)
            {
                spawnedInstances[i] = null;
                return;
            }
        }
    }

    // Query helpers
    public int GetAliveCount()
    {
        int alive = 0;
        foreach (var g in spawnedInstances) if (g != null) alive++;
        return alive;
    }

    public int GetDeadCount()
    {
        EnsureSpawnedInstancesListSize();
        int dead = 0;
        int maxIndex = Mathf.Min(enemyPrefabs.Count, spawnPoints.Count);
        for (int i = 0; i < maxIndex; i++)
        {
            if (spawnedInstances[i] == null) dead++;
        }
        return dead;
    }
}
