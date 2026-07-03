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
    [SerializeField] private StageMap stageMap;

    [Header("페이즈 설정 (startTime 오름차순)")]
    [SerializeField] private WavePhaseData[] phases;

    public int CurrentPhaseIndex { get; private set; } = -1;
    public WavePhaseData CurrentPhase => IsValidIndex(CurrentPhaseIndex) ? ActivePhases[CurrentPhaseIndex] : null;
    public int AliveCount { get; private set; }
    public StageMap Stage => stageMap;

    public event Action<int> OnPhaseChanged;
    public event Action OnBossWarning;
    public event Action OnBossTimeReached;

    private int weightSum;
    private Camera mainCam;
    private bool bossWarningRaised;
    private bool bossTimeReached;
    private int nextEliteSpawnIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        WavePhaseData[] activePhases = ActivePhases;
        if (activePhases == null || activePhases.Length == 0)
        {
            Debug.LogError("[WaveManager] phases 배열이 비어있습니다. Inspector에서 설정하세요.");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        TrySpawnDueEliteEnemies();
        TryRaiseBossTimeReached();
    }

    public void Configure(StageMap map)
    {
        stageMap = map;
        CurrentPhaseIndex = -1;
        weightSum = 0;
        bossWarningRaised = false;
        bossTimeReached = false;
        nextEliteSpawnIndex = 0;
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
        WavePhaseData[] activePhases = ActivePhases;

        // 현재 시간보다 startTime이 작거나 같은 것 중 가장 마지막 페이즈 선택
        int newIdx = 0;
        for (int i = 1; i < activePhases.Length; i++)
        {
            if (activePhases[i].startTime <= t) newIdx = i;
            else break; 
        }

        if (newIdx == CurrentPhaseIndex) return;

        CurrentPhaseIndex = newIdx;
        RebuildWeightSum(activePhases[newIdx]);
        SpawnEnterBurst(activePhases[newIdx]);
        OnPhaseChanged?.Invoke(newIdx);
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

        float h = mainCam.orthographicSize + ActiveSpawnMargin;
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
        return Mathf.Max(0f, ActivePhases[CurrentPhaseIndex + 1].startTime - SurvivedTime());
    }

    private void TryRaiseBossTimeReached()
    {
        if (bossTimeReached || stageMap == null)
            return;

        float survivedTime = SurvivedTime();
        TryRaiseBossWarning(survivedTime);

        if (survivedTime < stageMap.DurationSeconds)
            return;

        bossTimeReached = true;
        SpawnStageEnemy(stageMap.BossType, false, true);
        OnBossTimeReached?.Invoke();
    }

    private void TryRaiseBossWarning(float survivedTime)
    {
        if (bossWarningRaised || stageMap == null)
            return;

        if (stageMap.DurationSeconds - survivedTime > 10f)
            return;

        bossWarningRaised = true;
        OnBossWarning?.Invoke();
    }

    private void TrySpawnDueEliteEnemies()
    {
        if (stageMap == null || stageMap.EliteSpawnTimes == null)
            return;

        while (nextEliteSpawnIndex < stageMap.EliteSpawnTimes.Length
            && SurvivedTime() >= stageMap.EliteSpawnTimes[nextEliteSpawnIndex])
        {
            if (!SpawnStageEnemy(stageMap.EliteEnemyType, true, false))
                return;

            nextEliteSpawnIndex++;
        }
    }

    private bool SpawnStageEnemy(PoolType type, bool isElite, bool isBoss)
    {
        if (GameRoot.Instance?.Pool == null)
            return false;

        var go = GameRoot.Instance.Pool.Spawn(type, GetCameraEdgePosition(), Quaternion.identity);
        if (go == null)
            return false;

        AliveCount++;
        go.GetComponent<EnemyLifeHook>()?.Bind(this);

        if (isElite)
        {
            EliteEnemy elite = go.GetComponent<EliteEnemy>();
            if (elite == null)
                elite = go.AddComponent<EliteEnemy>();

            elite.Configure(stageMap.OccupationDurationSeconds);
        }

        if (isBoss)
        {
            BossEnemy boss = go.GetComponent<BossEnemy>();
            if (boss == null)
                boss = go.AddComponent<BossEnemy>();

            boss.Configure();
        }

        return true;
    }

    private void SpawnEnterBurst(WavePhaseData phase)
    {
        if (phase == null || phase.enterBurstCount <= 0)
            return;

        if (GameRoot.Instance?.Pool == null)
            return;

        int availableSlots = Mathf.Max(0, phase.maxAlive - AliveCount);
        int count = Mathf.Min(phase.enterBurstCount, availableSlots);

        for (int i = 0; i < count; i++)
        {
            var go = GameRoot.Instance.Pool.Spawn(phase.enterBurstType, GetCameraEdgePosition(), Quaternion.identity);
            if (go == null)
                continue;

            AliveCount++;
            go.GetComponent<EnemyLifeHook>()?.Bind(this);
        }
    }

    private WavePhaseData[] ActivePhases
    {
        get
        {
            if (stageMap != null && stageMap.Phases != null && stageMap.Phases.Length > 0)
                return stageMap.Phases;

            return phases;
        }
    }

    private float ActiveSpawnMargin => stageMap != null ? stageMap.SpawnMargin : spawnMargin;
    private bool IsValidIndex(int idx) => ActivePhases != null && idx >= 0 && idx < ActivePhases.Length;
    private float SurvivedTime() => GameRoot.Instance?.Game.SurvivedTime ?? 0f;
    private bool IsGameOver() => GameRoot.Instance?.Game.IsGameOver ?? false;
}
