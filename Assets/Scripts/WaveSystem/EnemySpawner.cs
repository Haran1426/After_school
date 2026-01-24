using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemySpawnData
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0f, 50f)] private float chance = 0f;

    public float Weight { get; set; }
    public GameObject Prefab => prefab;
    public float Chance => chance;
}
    
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnData[] enemies;

    [SerializeField, Range(0, 5000)]
    private int maxSpawnCount = 100;

    [SerializeField] private Vector2 min = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 max = new Vector2(8f, 4f);

    private float accumulatedWeight;

    private void Awake()
    {
        CalculateWeights();
    }

    private IEnumerator Start()
    {
        int count = 0;

        while (count < maxSpawnCount)
        {
            SpawnEnemy(GetRandomPosition());
            count++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void CalculateWeights()
    {
        accumulatedWeight = 0f;

        foreach (var enemy in enemies)
        {
            accumulatedWeight += enemy.Chance;
            enemy.Weight = accumulatedWeight;
        }
    }

    private void SpawnEnemy(Vector2 position)
    {
        var enemyData = enemies[GetRandomIndex()];
        Instantiate(enemyData.Prefab, position, Quaternion.identity);
    }

    private int GetRandomIndex()
    {
        float random = Random.value * accumulatedWeight;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].Weight >= random)
                return i;
        }

        return 0;
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
}
