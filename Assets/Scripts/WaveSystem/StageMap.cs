using UnityEngine;

public enum StageMapType
{
    Infinite,
    Bounded
}

public sealed class StageMap : MonoBehaviour
{
    [Header("Stage")]
    [SerializeField] private string stageName = "Green Forest";

    [TextArea]
    [SerializeField] private string description;

    [SerializeField, Min(1f)] private float durationSeconds = 600f;

    [Header("Map")]
    [SerializeField] private StageMapType mapType = StageMapType.Infinite;
    [SerializeField] private Vector2 boundsMin = new Vector2(-20f, -12f);
    [SerializeField] private Vector2 boundsMax = new Vector2(20f, 12f);

    [Header("Spawning")]
    [SerializeField, Min(0f)] private float spawnMargin = 1f;
    [SerializeField] private PoolType eliteEnemyType = PoolType.EnemyMelee;
    [SerializeField] private float[] eliteSpawnTimes = { 120f, 480f };
    [SerializeField] private PoolType bossType = PoolType.EnemyMelee;

    [Header("Occupation")]
    [SerializeField, Min(0.1f)] private float occupationDurationSeconds = 10f;

    [Header("Waves")]
    [SerializeField] private WavePhaseData[] phases;

    public string StageName => stageName;
    public string Description => description;
    public float DurationSeconds => durationSeconds;
    public StageMapType MapType => mapType;
    public Vector2 BoundsMin => boundsMin;
    public Vector2 BoundsMax => boundsMax;
    public float SpawnMargin => spawnMargin;
    public PoolType EliteEnemyType => eliteEnemyType;
    public float[] EliteSpawnTimes => eliteSpawnTimes;
    public PoolType BossType => bossType;
    public float OccupationDurationSeconds => occupationDurationSeconds;
    public WavePhaseData[] Phases => phases;
    public bool UsesMovementBounds => mapType == StageMapType.Bounded;

    public bool HasValidBounds()
    {
        return boundsMax.x > boundsMin.x && boundsMax.y > boundsMin.y;
    }

    private void OnDrawGizmosSelected()
    {
        if (!UsesMovementBounds || !HasValidBounds())
            return;

        Gizmos.color = Color.green;
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) * 0.5f, (boundsMin.y + boundsMax.y) * 0.5f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
