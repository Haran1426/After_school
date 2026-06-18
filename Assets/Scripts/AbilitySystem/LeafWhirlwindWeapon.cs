using System.Collections.Generic;
using UnityEngine;

public sealed class LeafWhirlwindWeapon : WeaponBase
{
    [SerializeField, Min(0.1f)] private float pulseInterval = 1.4f;
    [SerializeField, Min(0.1f)] private float radius = 2.4f;
    [SerializeField, Min(0f)] private float damage = 8f;
    [SerializeField, Min(0.1f)] private float effectLifetime = 0.32f;
    [SerializeField, Min(1)] private int leafCount = 10;
    [SerializeField] private Color leafColor = new Color(0.47f, 0.9f, 0.34f, 0.78f);

    private readonly List<EnemyBase> hitBuffer = new();
    private float nextPulseTime;
    private Player player;
    private Sprite leafSprite;

    private void Awake()
    {
        owner = transform;
        player = GetComponentInParent<Player>();
        leafSprite = CreateLeafSprite();
    }

    public override void Init(Transform ownerTransform)
    {
        base.Init(ownerTransform);
        player = ownerTransform.GetComponentInParent<Player>();
        nextPulseTime = Time.time + 0.2f;
    }

    private void Update()
    {
        if (owner == null)
            owner = transform;

        if (Time.time < nextPulseTime)
            return;

        Pulse();
        nextPulseTime = Time.time + pulseInterval;
    }

    protected override void OnLevelUp()
    {
        damage += 2.5f;
        radius += 0.18f;
        pulseInterval = Mathf.Max(0.45f, pulseInterval * 0.92f);
        leafCount = Mathf.Min(22, leafCount + 2);
    }

    private void Pulse()
    {
        Vector3 center = owner.position;
        float activeDamage = damage * (player != null ? Mathf.Max(0.1f, player.power) : 1f);

        hitBuffer.Clear();
        for (int i = 0; i < EnemyRegistry.All.Count; i++)
        {
            EnemyBase enemy = EnemyRegistry.All[i];
            if (enemy == null || enemy.IsDead)
                continue;

            float reach = radius + enemy.hitRadius;
            if ((enemy.transform.position - center).sqrMagnitude > reach * reach)
                continue;

            hitBuffer.Add(enemy);
        }

        for (int i = 0; i < hitBuffer.Count; i++)
            hitBuffer[i].TakeDamage(activeDamage);

        SpawnEffect(center);
        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.LeafWhirlwind);
    }

    private void SpawnEffect(Vector3 center)
    {
        for (int i = 0; i < leafCount; i++)
        {
            float angle = (360f / leafCount) * i + Random.Range(-12f, 12f);
            float distance = Random.Range(radius * 0.35f, radius);
            Vector3 offset = Quaternion.Euler(0f, 0f, angle) * Vector3.right * distance;

            GameObject leaf = new GameObject("LeafWhirlwindFx");
            leaf.transform.position = center + offset;
            leaf.transform.localScale = Vector3.one * Random.Range(0.18f, 0.32f);
            leaf.transform.rotation = Quaternion.Euler(0f, 0f, angle + Random.Range(25f, 80f));

            SpriteRenderer renderer = leaf.AddComponent<SpriteRenderer>();
            renderer.sprite = leafSprite;
            renderer.color = leafColor;
            renderer.sortingOrder = 12;

            LeafWhirlwindFx fx = leaf.AddComponent<LeafWhirlwindFx>();
            fx.Play(center, angle, effectLifetime);
        }
    }

    private static Sprite CreateLeafSprite()
    {
        const int width = 24;
        const int height = 16;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color leaf = Color.white;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x - width * 0.5f) / (width * 0.5f);
                float ny = (y - height * 0.5f) / (height * 0.5f);
                bool inside = nx * nx + ny * ny * 1.8f <= 1f && x > 2;
                texture.SetPixel(x, y, inside ? leaf : clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 24f);
    }
}

public sealed class LeafWhirlwindFx : MonoBehaviour
{
    private Vector3 center;
    private float angle;
    private float lifetime;
    private float age;
    private SpriteRenderer spriteRenderer;

    public void Play(Vector3 centerPosition, float startAngle, float duration)
    {
        center = centerPosition;
        angle = startAngle;
        lifetime = Mathf.Max(0.05f, duration);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = Mathf.Clamp01(age / lifetime);
        angle += 260f * Time.deltaTime;

        Vector3 radial = transform.position - center;
        float distance = radial.magnitude + 0.8f * Time.deltaTime;
        transform.position = center + Quaternion.Euler(0f, 0f, angle) * Vector3.right * distance;
        transform.localScale *= 1f + 1.5f * Time.deltaTime;

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
