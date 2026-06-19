using System.Collections.Generic;
using UnityEngine;

public sealed class GameJuiceFX : MonoBehaviour
{
    private const int MaxPooledParticles = 128;

    private static GameJuiceFX instance;
    private static Sprite circleSprite;
    private static Sprite streakSprite;

    private readonly Queue<JuiceParticle> particles = new Queue<JuiceParticle>();
    private Transform particleRoot;
    private Camera mainCamera;
    private Vector3 lastShakeOffset;
    private float shakeTimer;
    private float shakeDuration;
    private float shakeStrength;

    public static void EnsureExists()
    {
        if (instance != null || FindAnyObjectByType<GameJuiceFX>() != null)
            return;

        GameObject root = new GameObject("GameJuiceFX");
        root.AddComponent<GameJuiceFX>();
    }

    public static void HitSpark(Vector3 position, Vector3 direction, Color color, int count = 7)
    {
        EnsureExists();
        Sprite sprite = GetStreakSprite();
        Vector2 baseDir = direction.sqrMagnitude > 0.001f ? direction.normalized : Random.insideUnitCircle.normalized;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = (baseDir + Random.insideUnitCircle * 0.65f).normalized;
            float speed = Random.Range(2.6f, 6.2f);
            SpawnParticle(position, sprite, dir * speed, Random.Range(0.12f, 0.22f), Random.Range(0.18f, 0.34f), 0.02f, color, true);
        }
    }

    public static void ExpTrail(Vector3 position)
    {
        EnsureExists();
        Color color = new Color(0.45f, 1f, 0.72f, 0.72f);
        SpawnParticle(position, GetCircleSprite(), Random.insideUnitCircle * Random.Range(0.25f, 0.75f), 0.18f, 0.12f, 0.02f, color, false);
    }

    public static void ExpBurst(Vector3 position)
    {
        EnsureExists();
        Sprite sprite = GetCircleSprite();
        for (int i = 0; i < 10; i++)
        {
            Vector2 dir = Quaternion.Euler(0f, 0f, 36f * i + Random.Range(-9f, 9f)) * Vector2.right;
            Color color = Color.Lerp(new Color(0.42f, 1f, 0.55f, 0.9f), new Color(0.9f, 1f, 0.35f, 0.9f), Random.value);
            SpawnParticle(position, sprite, dir * Random.Range(1.2f, 2.6f), Random.Range(0.18f, 0.28f), Random.Range(0.12f, 0.24f), 0.01f, color, false);
        }
    }

    public static void Shake(float duration, float strength)
    {
        EnsureExists();
        if (instance == null)
            return;

        instance.shakeDuration = Mathf.Max(instance.shakeDuration, duration);
        instance.shakeTimer = Mathf.Max(instance.shakeTimer, duration);
        instance.shakeStrength = Mathf.Max(instance.shakeStrength, strength);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        particleRoot = new GameObject("FX Particles").transform;
        particleRoot.SetParent(transform, false);
    }

    private void LateUpdate()
    {
        UpdateCameraShake();
    }

    private void UpdateCameraShake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        if (lastShakeOffset != Vector3.zero)
        {
            mainCamera.transform.position -= lastShakeOffset;
            lastShakeOffset = Vector3.zero;
        }

        if (shakeTimer <= 0f)
            return;

        shakeTimer = Mathf.Max(0f, shakeTimer - Time.unscaledDeltaTime);
        float t = shakeDuration > 0f ? shakeTimer / shakeDuration : 0f;
        float strength = shakeStrength * t * t;
        lastShakeOffset = (Vector3)(Random.insideUnitCircle * strength);
        mainCamera.transform.position += lastShakeOffset;

        if (shakeTimer <= 0f)
        {
            shakeDuration = 0f;
            shakeStrength = 0f;
        }
    }

    private static void SpawnParticle(Vector3 position, Sprite sprite, Vector2 velocity, float lifetime, float startScale, float endScale, Color color, bool alignToVelocity)
    {
        EnsureExists();
        if (instance == null)
            return;

        JuiceParticle particle = instance.RentParticle();
        Transform particleTransform = particle.transform;
        particleTransform.position = position + (Vector3)(Random.insideUnitCircle * 0.08f);
        particleTransform.localScale = Vector3.one * startScale;
        particleTransform.rotation = Quaternion.identity;

        if (alignToVelocity && velocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            particleTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        SpriteRenderer renderer = particle.Renderer;
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = 45;

        particle.Play(velocity, lifetime, startScale, endScale, color);
    }

    private JuiceParticle RentParticle()
    {
        JuiceParticle particle = particles.Count > 0 ? particles.Dequeue() : CreateParticle();
        particle.gameObject.SetActive(true);
        return particle;
    }

    private JuiceParticle CreateParticle()
    {
        GameObject particleObject = new GameObject("JuiceParticle");
        particleObject.transform.SetParent(particleRoot != null ? particleRoot : transform, false);

        SpriteRenderer renderer = particleObject.AddComponent<SpriteRenderer>();
        JuiceParticle particle = particleObject.AddComponent<JuiceParticle>();
        particle.Initialize(this, renderer);
        return particle;
    }

    public void ReturnParticle(JuiceParticle particle)
    {
        if (particle == null)
            return;

        if (particles.Count >= MaxPooledParticles)
        {
            Destroy(particle.gameObject);
            return;
        }

        particle.gameObject.SetActive(false);
        particle.transform.SetParent(particleRoot != null ? particleRoot : transform, false);
        particles.Enqueue(particle);
    }

    private static Sprite GetCircleSprite()
    {
        if (circleSprite != null)
            return circleSprite;

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.42f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(d / radius));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }

    private static Sprite GetStreakSprite()
    {
        if (streakSprite != null)
            return streakSprite;

        const int width = 48;
        const int height = 12;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2(width * 0.35f, (height - 1) * 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Abs(x - center.x) / (width * 0.5f);
                float dy = Mathf.Abs(y - center.y) / (height * 0.5f);
                float alpha = Mathf.Clamp01((1f - dx) * (1f - dy * dy));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        streakSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.15f, 0.5f), 32f);
        return streakSprite;
    }
}

public sealed class JuiceParticle : MonoBehaviour
{
    public SpriteRenderer Renderer => spriteRenderer;

    private GameJuiceFX owner;
    private Vector2 velocity;
    private float lifetime;
    private float age;
    private float startScale;
    private float endScale;
    private Color startColor;
    private SpriteRenderer spriteRenderer;

    public void Initialize(GameJuiceFX particleOwner, SpriteRenderer renderer)
    {
        owner = particleOwner;
        spriteRenderer = renderer;
    }

    public void Play(Vector2 startVelocity, float duration, float scaleFrom, float scaleTo, Color color)
    {
        velocity = startVelocity;
        lifetime = Mathf.Max(0.03f, duration);
        age = 0f;
        startScale = scaleFrom;
        endScale = scaleTo;
        startColor = color;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = Mathf.Clamp01(age / lifetime);
        transform.position += (Vector3)(velocity * Time.deltaTime);
        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * 5f);
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
        transform.Rotate(0f, 0f, 220f * Time.deltaTime);

        if (spriteRenderer != null)
        {
            Color color = startColor;
            color.a *= 1f - t;
            spriteRenderer.color = color;
        }

        if (age >= lifetime)
        {
            if (owner != null)
                owner.ReturnParticle(this);
            else
                Destroy(gameObject);
        }
    }
}
