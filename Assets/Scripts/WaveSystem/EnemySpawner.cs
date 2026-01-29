using System.Collections;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public PoolType type;
    public int weight = 1;
}

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private SpawnEntry[] enemies;

    [SerializeField] private float interval = 1f;
    [SerializeField] private int spawnPerTick = 1;
    [SerializeField] private int maxAlive = 50;

    private int alive;
    private int weightSum;

    private void Awake()
    {
        for (int i = 0; i < enemies.Length; i++)
            weightSum += Mathf.Max(0, enemies[i].weight);
    }

    private void OnEnable()
    {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        var wait = new WaitForSeconds(interval);

        while (true)
        {
            for (int i = 0; i < spawnPerTick; i++)
            {
                if (alive >= maxAlive) break;
                SpawnOne();
            }

            yield return wait;
        }
    }
    private void SpawnOne()
    {
        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var type = PickType();

        var go = PoolManager.Instance.Spawn(type, point.position, point.rotation);
        if (go == null) return;

        alive++;

        var po = go.GetComponent<PooledObject>();
        if (po != null)
        {
            go.GetComponent<EnemyLifeHook>()?.Bind(this);
        }
    }

    public void NotifyEnemyDead()
    {
        alive = Mathf.Max(0, alive - 1);
    }

    private PoolType PickType()
    {
        int r = Random.Range(0, weightSum);
        int acc = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            acc += enemies[i].weight;
            if (r < acc) return enemies[i].type;
        }

        return enemies[0].type;
    }
}
