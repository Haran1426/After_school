using UnityEngine;

public sealed class EliteEnemy : MonoBehaviour
{
    [SerializeField] private OccupationZone occupationZonePrefab;
    [SerializeField, Min(0.1f)] private float occupationSeconds = 10f;
    [SerializeField, Min(0.1f)] private float generatedZoneRadius = 2.5f;

    private EnemyBase enemy;
    private bool isArmed;
    private bool spawnedZone;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    private void OnEnable()
    {
        spawnedZone = false;

        if (enemy == null)
            enemy = GetComponent<EnemyBase>();

        if (enemy != null)
            enemy.Died += HandleEnemyDied;
    }

    private void OnDisable()
    {
        isArmed = false;

        if (enemy != null)
            enemy.Died -= HandleEnemyDied;
    }

    public void Configure(float staySeconds)
    {
        occupationSeconds = Mathf.Max(0.1f, staySeconds);
        spawnedZone = false;
        isArmed = true;
    }

    private void HandleEnemyDied(EnemyBase deadEnemy)
    {
        if (!isArmed || spawnedZone)
            return;

        spawnedZone = true;
        isArmed = false;
        SpawnOccupationZone(deadEnemy.transform.position);
    }

    private void SpawnOccupationZone(Vector3 position)
    {
        OccupationZone zone;

        if (occupationZonePrefab != null)
        {
            zone = Instantiate(occupationZonePrefab, position, Quaternion.identity);
        }
        else
        {
            GameObject go = new GameObject("OccupationZone");
            go.transform.position = position;

            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.radius = generatedZoneRadius;

            zone = go.AddComponent<OccupationZone>();
        }

        zone.Configure(occupationSeconds);
    }
}
