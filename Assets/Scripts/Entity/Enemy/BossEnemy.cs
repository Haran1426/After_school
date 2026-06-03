using UnityEngine;

public sealed class BossEnemy : MonoBehaviour
{
    public static BossEnemy ActiveBoss { get; private set; }

    [SerializeField, Min(1f)] private float maxHpMultiplier = 10f;
    [SerializeField, Min(1f)] private float scaleMultiplier = 2.2f;
    [SerializeField] private Color bossTint = new Color(0.78f, 0.22f, 0.16f, 1f);
    [Header("Boss Pattern")]
    [SerializeField, Min(1f)] private float shockwaveInterval = 5.5f;
    [SerializeField, Min(0.1f)] private float shockwaveWarningSeconds = 0.75f;
    [SerializeField, Min(0.1f)] private float shockwaveRadius = 3.4f;
    [SerializeField, Min(0f)] private float shockwaveDamage = 2f;
    [SerializeField] private Color warningColor = new Color(1f, 0.62f, 0.2f, 1f);

    private EnemyBase enemy;
    private SpriteRenderer spriteRenderer;
    private bool isArmed;
    private bool hasBaseStats;
    private float baseMaxHp;
    private Vector3 baseScale;
    private Color baseColor;
    private float nextShockwaveTime;
    private float shockwaveReadyTime;
    private bool isChargingShockwave;

    public bool IsActiveBoss => isArmed && enemy != null && !enemy.IsDead;
    public float Health01 => enemy != null && enemy.maxHp > 0f ? Mathf.Clamp01(enemy.currentHp / enemy.maxHp) : 0f;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CacheBaseStats();
    }

    private void OnEnable()
    {
        if (enemy == null)
            enemy = GetComponent<EnemyBase>();

        if (enemy != null)
            enemy.Died += HandleEnemyDied;
    }

    private void Update()
    {
        if (!IsActiveBoss || GameRoot.Instance?.Game?.IsGameOver == true || GameRoot.Instance?.Game?.IsStageCleared == true)
            return;

        UpdateShockwavePattern();
    }

    private void OnDisable()
    {
        if (ActiveBoss == this)
            ActiveBoss = null;

        isArmed = false;
        isChargingShockwave = false;
        RestoreBaseStats();

        if (enemy != null)
            enemy.Died -= HandleEnemyDied;
    }

    public void Configure()
    {
        CacheBaseStats();
        ApplyBossStats();
        ActiveBoss = this;
        isArmed = true;
        isChargingShockwave = false;
        nextShockwaveTime = Time.time + shockwaveInterval * 0.65f;
    }

    private void HandleEnemyDied(EnemyBase deadEnemy)
    {
        if (!isArmed)
            return;

        isArmed = false;
        ActiveBoss = null;
        GameRoot.Instance?.Game?.StageClear();
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

    private void ApplyBossStats()
    {
        if (enemy == null)
            return;

        enemy.maxHp = baseMaxHp * maxHpMultiplier;
        enemy.currentHp = enemy.maxHp;
        transform.localScale = baseScale * scaleMultiplier;

        if (spriteRenderer != null)
            spriteRenderer.color = bossTint;
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

    private void UpdateShockwavePattern()
    {
        if (!isChargingShockwave)
        {
            if (Time.time < nextShockwaveTime)
                return;

            BeginShockwaveWarning();
            return;
        }

        float t = Mathf.Clamp01(1f - ((shockwaveReadyTime - Time.time) / shockwaveWarningSeconds));
        transform.localScale = baseScale * scaleMultiplier * (1f + Mathf.Sin(t * Mathf.PI) * 0.08f);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.Lerp(bossTint, warningColor, 0.45f + Mathf.Sin(Time.time * 18f) * 0.25f);

        if (Time.time < shockwaveReadyTime)
            return;

        FireShockwave();
    }

    private void BeginShockwaveWarning()
    {
        isChargingShockwave = true;
        shockwaveReadyTime = Time.time + shockwaveWarningSeconds;
        SpawnShockwaveRing(shockwaveRadius, shockwaveWarningSeconds, warningColor, 0.38f);
        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.BossShockwave);
    }

    private void FireShockwave()
    {
        isChargingShockwave = false;
        nextShockwaveTime = Time.time + shockwaveInterval;
        transform.localScale = baseScale * scaleMultiplier;

        if (spriteRenderer != null)
            spriteRenderer.color = bossTint;

        SpawnShockwaveRing(shockwaveRadius, 0.42f, new Color(1f, 0.2f, 0.08f, 1f), 0.62f);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
            return;

        float distance = Vector2.Distance(transform.position, playerObject.transform.position);
        if (distance > shockwaveRadius)
            return;

        Player player = playerObject.GetComponent<Player>();
        player?.TakeDamage(shockwaveDamage);
    }

    private void SpawnShockwaveRing(float radius, float lifetime, Color color, float alpha)
    {
        GameObject ring = new GameObject("BossShockwaveRing");
        ring.transform.position = transform.position;
        ring.transform.localScale = Vector3.one * radius * 2f;

        SpriteRenderer renderer = ring.AddComponent<SpriteRenderer>();
        color.a = alpha;
        renderer.color = color;
        renderer.sprite = BossShockwaveRing.CreateRingSprite();
        renderer.sortingOrder = 14;

        BossShockwaveRing effect = ring.AddComponent<BossShockwaveRing>();
        effect.Play(lifetime);
    }
}

public sealed class BossShockwaveRing : MonoBehaviour
{
    private static Sprite ringSprite;

    private SpriteRenderer spriteRenderer;
    private float lifetime;
    private float age;
    private Vector3 startScale;

    public static Sprite CreateRingSprite()
    {
        if (ringSprite != null)
            return ringSprite;

        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float outer = size * 0.45f;
        float inner = size * 0.36f;
        Color clear = new Color(1f, 1f, 1f, 0f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                float edge = Mathf.InverseLerp(outer, inner, d);
                float alpha = d <= outer && d >= inner ? Mathf.Clamp01(edge) : 0f;
                texture.SetPixel(x, y, alpha > 0f ? new Color(1f, 1f, 1f, alpha) : clear);
            }
        }

        texture.Apply();
        ringSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return ringSprite;
    }

    public void Play(float duration)
    {
        lifetime = Mathf.Max(0.05f, duration);
        spriteRenderer = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = Mathf.Clamp01(age / lifetime);
        transform.localScale = Vector3.Lerp(startScale * 0.18f, startScale, t);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(color.a, 0f, t);
            spriteRenderer.color = color;
        }

        if (age >= lifetime)
            Destroy(gameObject);
    }
}
