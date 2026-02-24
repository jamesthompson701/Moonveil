using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central coordinator that prevents every enemy from attacking simultaneously.
/// Enemies report they are "ready to attack"; the director grants a limited number of temporary permits.
/// </summary>
public class EnemyAttackDirector : MonoBehaviour
{
    public static EnemyAttackDirector Instance { get; private set; }

    [Header("Attack Pacing")]
    [Tooltip("Max number of enemies allowed to be attacking (windup/execution) at the same time.")]
    [SerializeField, Range(1, 8)] private int maxConcurrentAttackers = 2;

    [Tooltip("How often the director re-evaluates and hands out attack permits (seconds).")]
    [SerializeField, Min(0.05f)] private float reevaluateInterval = 0.25f;

    [Tooltip("How long an enemy keeps a permit to start an attack (seconds). If it doesn't start in time, permit expires.")]
    [SerializeField, Min(0.05f)] private float permitDuration = 0.6f;

    [Tooltip("Minimum time between new attack starts (seconds). Helps avoid 'instant dogpile'.")]
    [SerializeField, Min(0f)] private float minTimeBetweenAttackStarts = 0.15f;

    [Header("Scoring")]
    [Tooltip("Weight on distance (closer enemies are favored).")]
    [SerializeField, Min(0f)] private float distanceWeight = 1.0f;

    [Tooltip("Weight on 'time since last attack' (prevents the same enemy always attacking).")]
    [SerializeField, Min(0f)] private float fairnessWeight = 0.7f;

    [Tooltip("Weight on randomness (adds variety).")]
    [SerializeField, Min(0f)] private float randomWeight = 0.35f;

    private readonly Dictionary<int, Candidate> _candidates = new Dictionary<int, Candidate>(64);
    private readonly HashSet<int> _attacking = new HashSet<int>();
    private readonly Dictionary<int, float> _permitUntil = new Dictionary<int, float>(64);
    private readonly Dictionary<int, float> _lastAttackTime = new Dictionary<int, float>(64);

    private readonly List<ScoredCandidate> _scored = new List<ScoredCandidate>(64);

    private float _nextReevalTime;
    private float _nextGlobalAttackStartTime;

    private struct Candidate
    {
        public CreatureDefs Enemy;
        public float Distance;
        public float LastReportTime;
    }

    private struct ScoredCandidate
    {
        public CreatureDefs Enemy;
        public float Score;
    }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Time.time < _nextReevalTime) return;

        _nextReevalTime = Time.time + reevaluateInterval;

        CleanupExpiredPermits();
        GrantPermits();
    }

    public void ReportReadyToAttack(CreatureDefs enemy, float distanceToTarget)
    {
        if (!enemy) return;

        int id = enemy.GetInstanceID();

        _candidates[id] = new Candidate
        {
            Enemy = enemy,
            Distance = Mathf.Max(0.01f, distanceToTarget),
            LastReportTime = Time.time
        };
    }

    public bool CanStartAttack(CreatureDefs enemy)
    {
        if (!enemy) return false;

        int id = enemy.GetInstanceID();

        if (_attacking.Contains(id)) return true;

        if (_permitUntil.TryGetValue(id, out float until))
            return Time.time <= until;

        return false;
    }

    public bool TryBeginAttack(CreatureDefs enemy)
    {
        if (!enemy) return false;

        int id = enemy.GetInstanceID();

        if (_attacking.Contains(id)) return true;

        if (_attacking.Count >= maxConcurrentAttackers) return false;
        if (Time.time < _nextGlobalAttackStartTime) return false;

        if (!_permitUntil.TryGetValue(id, out float until) || Time.time > until)
            return false;

        _permitUntil.Remove(id);
        _attacking.Add(id);
        _lastAttackTime[id] = Time.time;

        _nextGlobalAttackStartTime = Time.time + minTimeBetweenAttackStarts;
        return true;
    }

    public void EndAttack(CreatureDefs enemy)
    {
        if (!enemy) return;
        _attacking.Remove(enemy.GetInstanceID());
    }

    private void CleanupExpiredPermits()
    {
        var staleIds = ListPool<int>.Get();

        foreach (var kvp in _candidates)
        {
            if (!kvp.Value.Enemy || (Time.time - kvp.Value.LastReportTime) > 1.0f)
                staleIds.Add(kvp.Key);
        }
        foreach (int id in staleIds) _candidates.Remove(id);
        ListPool<int>.Release(staleIds);

        staleIds = ListPool<int>.Get();
        foreach (var kvp in _permitUntil)
        {
            if (Time.time > kvp.Value)
                staleIds.Add(kvp.Key);
        }
        foreach (int id in staleIds) _permitUntil.Remove(id);
        ListPool<int>.Release(staleIds);
    }

    private void GrantPermits()
    {
        int openSlots = maxConcurrentAttackers - _attacking.Count;
        if (openSlots <= 0) return;

        _scored.Clear();

        foreach (var kvp in _candidates)
        {
            CreatureDefs e = kvp.Value.Enemy;
            if (!e) continue;

            int id = e.GetInstanceID();
            if (_attacking.Contains(id)) continue;
            if (_permitUntil.ContainsKey(id)) continue;

            float distance = kvp.Value.Distance;

            float distanceScore = 1f / distance;
            float timeSinceLast = Time.time - (_lastAttackTime.TryGetValue(id, out float t) ? t : -999f);
            float fairnessScore = Mathf.Clamp01(timeSinceLast / 5f);
            float randomScore = Random.value;

            float score =
                (distanceScore * distanceWeight) +
                (fairnessScore * fairnessWeight) +
                (randomScore * randomWeight);

            _scored.Add(new ScoredCandidate { Enemy = e, Score = score });
        }

        _scored.Sort((a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < _scored.Count && openSlots > 0; i++)
        {
            CreatureDefs e = _scored[i].Enemy;
            if (!e) continue;

            int id = e.GetInstanceID();
            _permitUntil[id] = Time.time + permitDuration;
            openSlots--;
        }
    }

    private static class ListPool<T>
    {
        private static readonly Stack<List<T>> Pool = new Stack<List<T>>(16);

        public static List<T> Get()
        {
            if (Pool.Count > 0)
            {
                var list = Pool.Pop();
                list.Clear();
                return list;
            }
            return new List<T>(16);
        }

        public static void Release(List<T> list)
        {
            if (list == null) return;
            list.Clear();
            Pool.Push(list);
        }
    }
}
