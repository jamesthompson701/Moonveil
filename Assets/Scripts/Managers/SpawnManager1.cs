using System.Collections.Generic;
using UnityEngine;

public class SpawnManager1 : MonoBehaviour
{
    public static SpawnManager1 instance;

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

    public void SpawnAllMissing()
    {
        EnsureSpawnedInstancesListSize();

        int maxIndex = Mathf.Min(enemyPrefabs != null ? enemyPrefabs.Count : 0, spawnPoints != null ? spawnPoints.Count : 0);

        // Collect indices where we are missing an instance (dead slots)
        List<int> deadIndices = new List<int>();
        for (int i = 0; i < maxIndex; i++)
        {
            if (spawnPoints[i] == null) continue;
            if (spawnedInstances[i] == null) deadIndices.Add(i);
        }

        if (deadIndices.Count == 0) return;

        // Create a shuffled list of available spawn indices (same as deadIndices, but randomized)
        List<int> availableSpawnIndices = new List<int>(deadIndices);

        for (int i = availableSpawnIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = availableSpawnIndices[i];
            availableSpawnIndices[i] = availableSpawnIndices[j];
            availableSpawnIndices[j] = tmp;
        }

        int spawnCount = Mathf.Min(deadIndices.Count, availableSpawnIndices.Count);

        // For each dead prefab index, spawn that prefab at a random available spawn index (without replacement)
        for (int k = 0; k < spawnCount; k++)
        {
            int prefabIndex = deadIndices[k];
            int spawnIndex = availableSpawnIndices[k];

            // Bounds and null checks
            if (prefabIndex < 0 || prefabIndex >= enemyPrefabs.Count) continue;
            if (spawnIndex < 0 || spawnIndex >= spawnPoints.Count) continue;

            var prefab = enemyPrefabs[prefabIndex];
            var spawnPoint = spawnPoints[spawnIndex];
            if (prefab == null || spawnPoint == null) continue;

            GameObject inst = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedInstances[spawnIndex] = inst;
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
