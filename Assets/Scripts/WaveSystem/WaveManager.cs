using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 무한모드 웨이브 매니저.
/// </summary>
public sealed class WaveManager : MonoBehaviour, IAliveCounter
{
    public static WaveManager Instance { get; private set; }

    [Header("스폰 설정")]
    [Tooltip("카메라 경계에서 얼마나 바깥에 스폰할지 (유닛)")]
    [SerializeField] private float spawnMargin = 1f;

    [Header("페이즈 설정 (startTime 오름차순)")]
    [SerializeField] private WavePhaseData[] phases;

    public int CurrentPhaseIndex { get; private set; } = -1;
    public WavePhaseData CurrentPhase => IsValidIndex(CurrentPhaseIndex) ? phases[CurrentPhaseIndex] : null;
    public int AliveCount { get; private set; }

    public event Action<int> OnPhaseChanged;

    private int weightSum;
    private Camera mainCam;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        if (phases == null || phases.Length == 0)
        {
            Debug.LogError("[WaveManager] phases 배열이 비어있습니다. Inspector에서 설정하세요.");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    // ── 스폰 루프 ──────────────────────────────────────────────────────────
    private IEnumerator SpawnLoop()
    {
        WaitForSeconds wait = null;
        float cachedInterval = -1f;

        while (true)
        {
            if (IsGameOver()) yield break;

            TryAdvancePhase();

            WavePhaseData phase = CurrentPhase;
            if (phase == null) { yield return null; continue; }


            if (!Mathf.Approximately(phase.spawnInterval, cachedInterval))
            {
                wait = new WaitForSeconds(phase.spawnInterval);
                cachedInterval = phase.spawnInterval;
            }

            int count = Mathf.Min(phase.spawnPerTick, phase.maxAlive - AliveCount);
            for (int i = 0; i < count; i++)
                SpawnOne(phase);

            yield return wait;
        }
    }


    private void TryAdvancePhase()
    {
        float t = SurvivedTime();

        // 현재 시간보다 startTime이 작거나 같은 것 중 가장 마지막 페이즈 선택
        int newIdx = 0;
        for (int i = 1; i < phases.Length; i++)
        {
            if (phases[i].startTime <= t) newIdx = i;
            else break; 
        }

        if (newIdx == CurrentPhaseIndex) return;

        CurrentPhaseIndex = newIdx;
        RebuildWeightSum(phases[newIdx]);
        OnPhaseChanged?.Invoke(newIdx);

        Debug.Log($"[WaveManager] → {phases[newIdx].label} (phase {newIdx}, {phases[newIdx].startTime}s)");
    }

    private void RebuildWeightSum(WavePhaseData phase)
    {
        weightSum = 0;
        if (phase.enemies == null) return;
        foreach (var e in phase.enemies)
            weightSum += Mathf.Max(0, e.weight);
    }


    private void SpawnOne(WavePhaseData phase)
    {
        if (weightSum <= 0) return;
        if (GameRoot.Instance?.Pool == null) return;

        Vector3 pos = GetCameraEdgePosition();
        PoolType type = PickType(phase);

        var go = GameRoot.Instance.Pool.Spawn(type, pos, Quaternion.identity);
        if (go == null) return;

        AliveCount++;
        go.GetComponent<EnemyLifeHook>()?.Bind(this);
    }

    private PoolType PickType(WavePhaseData phase)
    {
        int r = UnityEngine.Random.Range(0, weightSum);
        int acc = 0;
        foreach (var e in phase.enemies)
        {
            acc += e.weight;
            if (r < acc) return e.type;
        }
        return phase.enemies[0].type;
    }

    private Vector3 GetCameraEdgePosition()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return Vector3.zero;

        float h = mainCam.orthographicSize + spawnMargin;
        float w = h * mainCam.aspect;

        Vector3 center = mainCam.transform.position;
        center.z = 0f;

        // 0=위, 1=아래, 2=왼쪽, 3=오른쪽
        return UnityEngine.Random.Range(0, 4) switch
        {
            0 => center + new Vector3(UnityEngine.Random.Range(-w, w),  h, 0f),
            1 => center + new Vector3(UnityEngine.Random.Range(-w, w), -h, 0f),
            2 => center + new Vector3(-w, UnityEngine.Random.Range(-h, h), 0f),
            _ => center + new Vector3( w, UnityEngine.Random.Range(-h, h), 0f),
        };
    }

    public void NotifyEnemyDead()
    {
        AliveCount = Mathf.Max(0, AliveCount - 1);
    }

    public float TimeUntilNextPhase()
    {
        if (!IsValidIndex(CurrentPhaseIndex + 1)) return -1f;
        return Mathf.Max(0f, phases[CurrentPhaseIndex + 1].startTime - SurvivedTime());
    }

    private bool IsValidIndex(int idx) => phases != null && idx >= 0 && idx < phases.Length;
    private float SurvivedTime() => GameRoot.Instance?.Game.SurvivedTime ?? 0f;
    private bool IsGameOver() => GameRoot.Instance?.Game.IsGameOver ?? false;
}
