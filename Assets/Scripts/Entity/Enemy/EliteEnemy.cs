using UnityEngine;

public sealed class EliteEnemy : MonoBehaviour
{
    [SerializeField] private OccupationZone occupationZonePrefab;
    [SerializeField, Min(0.1f)] private float occupationSeconds = 10f;
    [SerializeField, Min(0.1f)] private float generatedZoneRadius = 2.5f;
    [SerializeField, Min(1f)] private float maxHpMultiplier = 3f;
    [SerializeField, Min(1f)] private float scaleMultiplier = 1.35f;
    [SerializeField] private Color eliteTint = new Color(1f, 0.72f, 0.22f, 1f);
    [SerializeField] private Color auraColor = new Color(1f, 0.86f, 0.15f, 0.48f);

    private EnemyBase enemy;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer auraRenderer;
    private SpriteRenderer markerRenderer;
    private bool isArmed;
    private bool spawnedZone;
    private bool hasBaseStats;
    private float baseMaxHp;
    private Vector3 baseScale;
    private Color baseColor;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CacheBaseStats();
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
        SetEliteVisualsActive(false);
        RestoreBaseStats();

        if (enemy != null)
            enemy.Died -= HandleEnemyDied;
    }

    public void Configure(float staySeconds)
    {
        CacheBaseStats();
        occupationSeconds = Mathf.Max(0.1f, staySeconds);
        spawnedZone = false;
        isArmed = true;

        ApplyEliteStats();
        EnsureEliteVisuals();
        SetEliteVisualsActive(true);
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

    private void CacheBaseStats()
    {
        if (hasBaseStats)
            return;

        if (enemy == null)
            enemy = GetComponent<EnemyBase>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (enemy == null)
            return;

        baseMaxHp = enemy.maxHp;
        baseScale = transform.localScale;
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        hasBaseStats = true;
    }

    private void ApplyEliteStats()
    {
        if (enemy == null)
            return;

        enemy.maxHp = baseMaxHp * maxHpMultiplier;
        enemy.currentHp = enemy.maxHp;
        transform.localScale = baseScale * scaleMultiplier;

        if (spriteRenderer != null)
            spriteRenderer.color = eliteTint;
    }

    private void RestoreBaseStats()
    {
        if (!hasBaseStats || enemy == null)
            return;

        enemy.maxHp = baseMaxHp;
        transform.localScale = baseScale;

        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }

    private void Update()
    {
        if (!isArmed)
            return;

        if (auraRenderer != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 4.4f) * 0.08f;
            auraRenderer.transform.localScale = Vector3.one * pulse;
            Color color = auraColor;
            color.a = auraColor.a * (0.72f + Mathf.Sin(Time.time * 5.8f) * 0.18f);
            auraRenderer.color = color;
        }

        if (markerRenderer != null)
        {
            markerRenderer.transform.localPosition = new Vector3(0f, 0.88f + Mathf.Sin(Time.time * 5f) * 0.08f, 0f);
            markerRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 3f) * 6f);
        }
    }

    private void EnsureEliteVisuals()
    {
        if (auraRenderer == null)
        {
            GameObject aura = new GameObject("EliteAura");
            aura.transform.SetParent(transform, false);
            aura.transform.localPosition = Vector3.zero;
            aura.transform.localScale = Vector3.one;
            auraRenderer = aura.AddComponent<SpriteRenderer>();
            auraRenderer.sprite = EliteVisualSprites.GetRingSprite();
            auraRenderer.color = auraColor;
            auraRenderer.sortingOrder = 11;
        }

        if (markerRenderer == null)
        {
            GameObject marker = new GameObject("EliteMarker");
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = new Vector3(0f, 0.88f, 0f);
            marker.transform.localScale = Vector3.one * 0.36f;
            markerRenderer = marker.AddComponent<SpriteRenderer>();
            markerRenderer.sprite = EliteVisualSprites.GetCrownSprite();
            markerRenderer.color = new Color(1f, 0.92f, 0.28f, 1f);
            markerRenderer.sortingOrder = 30;
        }
    }

    private void SetEliteVisualsActive(bool active)
    {
        if (auraRenderer != null)
            auraRenderer.gameObject.SetActive(active);

        if (markerRenderer != null)
            markerRenderer.gameObject.SetActive(active);
    }
}

public static class EliteVisualSprites
{
    private static Sprite ringSprite;
    private static Sprite crownSprite;

    public static Sprite GetRingSprite()
    {
        if (ringSprite != null)
            return ringSprite;

        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float outer = size * 0.44f;
        float inner = size * 0.3f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                float alpha = d <= outer && d >= inner ? Mathf.SmoothStep(0f, 1f, (outer - d) / (outer - inner)) : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        ringSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return ringSprite;
    }

    public static Sprite GetCrownSprite()
    {
        if (crownSprite != null)
            return crownSprite;

        const int width = 32;
        const int height = 24;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color clear = new Color(1f, 1f, 1f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, y, clear);
        }

        DrawTriangle(texture, new Vector2(2f, 6f), new Vector2(9f, 21f), new Vector2(14f, 6f));
        DrawTriangle(texture, new Vector2(9f, 6f), new Vector2(16f, 23f), new Vector2(23f, 6f));
        DrawTriangle(texture, new Vector2(18f, 6f), new Vector2(25f, 21f), new Vector2(30f, 6f));

        for (int y = 3; y <= 8; y++)
        {
            for (int x = 3; x < width - 3; x++)
                texture.SetPixel(x, y, Color.white);
        }

        texture.Apply();
        crownSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.25f), 32f);
        return crownSprite;
    }

    private static void DrawTriangle(Texture2D texture, Vector2 a, Vector2 b, Vector2 c)
    {
        int minX = Mathf.FloorToInt(Mathf.Min(a.x, Mathf.Min(b.x, c.x)));
        int maxX = Mathf.CeilToInt(Mathf.Max(a.x, Mathf.Max(b.x, c.x)));
        int minY = Mathf.FloorToInt(Mathf.Min(a.y, Mathf.Min(b.y, c.y)));
        int maxY = Mathf.CeilToInt(Mathf.Max(a.y, Mathf.Max(b.y, c.y)));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInTriangle(p, a, b, c))
                    texture.SetPixel(x, y, Color.white);
            }
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Sign(p, a, b);
        float d2 = Sign(p, b, c);
        float d3 = Sign(p, c, a);
        bool hasNeg = d1 < 0f || d2 < 0f || d3 < 0f;
        bool hasPos = d1 > 0f || d2 > 0f || d3 > 0f;
        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
