using UnityEngine;

public interface IAliveCounter
{
    void NotifyEnemyDead();
}

[System.Serializable]
public class SpawnEntry
{
    public PoolType type;
    public int weight = 1;
}

[System.Serializable]
public class WavePhaseData
{
    [Tooltip("이 페이즈가 시작되는 게임 시간 (초)")]
    public float startTime = 0f;

    [Tooltip("Inspector에서 구분용 이름")]
    public string label = "Phase";

    [Tooltip("스폰 간격 (초)")]
    [Min(0.1f)] public float spawnInterval = 1f;

    [Tooltip("한 번에 스폰할 적 수")]
    [Min(1)] public int spawnPerTick = 1;

    [Tooltip("동시에 살아있을 수 있는 최대 적 수")]
    [Min(1)] public int maxAlive = 30;

    [Tooltip("스폰할 적 목록 (weight로 확률 조절)")]
    public SpawnEntry[] enemies;
}
