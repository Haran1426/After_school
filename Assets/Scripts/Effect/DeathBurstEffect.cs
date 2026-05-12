using UnityEngine;

public static class DeathBurstEffect
{
    private static Sprite squareSprite;
    private static Sprite circleSprite;

    private static readonly Color[] Palette =
    {
        new Color(1f, 0.28f, 0.22f),
        new Color(1f, 0.82f, 0.24f),
        new Color(0.28f, 0.82f, 0.88f),
        new Color(0.64f, 0.43f, 0.92f),
        new Color(1f, 0.96f, 0.82f),
    };

    public static void Spawn(Vector3 position)
    {
        int count = Random.Range(10, 15);

        for (int i = 0; i < count; i++)
        {
            var particle = new GameObject("DeathBurstParticle");
            particle.transform.position = position + (Vector3)(Random.insideUnitCircle * 0.12f);
            particle.transform.localScale = Vector3.one * Random.Range(0.08f, 0.15f);

            var renderer = particle.AddComponent<SpriteRenderer>();
            renderer.sprite = Random.value < 0.35f ? CircleSprite : SquareSprite;
            renderer.color = Palette[Random.Range(0, Palette.Length)];
            renderer.sortingOrder = 50;

            Vector2 direction = Random.insideUnitCircle;
            if (direction.sqrMagnitude < 0.01f)
                direction = Vector2.up;

            direction.Normalize();

            var mover = particle.AddComponent<DeathBurstParticle>();
            mover.Init(
                direction * Random.Range(1.7f, 3.8f),
                Random.Range(0.42f, 0.68f),
                Random.Range(-420f, 420f),
                Random.Range(1.6f, 2.8f)
            );
        }
    }

    private static Sprite SquareSprite
    {
        get
        {
            if (squareSprite == null)
                squareSprite = CreateSprite(false);

            return squareSprite;
        }
    }

    private static Sprite CircleSprite
    {
        get
        {
            if (circleSprite == null)
                circleSprite = CreateSprite(true);

            return circleSprite;
        }
    }

    private static Sprite CreateSprite(bool circle)
    {
        const int size = 16;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        var clear = new Color(0f, 0f, 0f, 0f);
        var white = Color.white;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool filled = !circle || Vector2.Distance(new Vector2(x, y), center) <= 6.5f;
                texture.SetPixel(x, y, filled ? white : clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

public class DeathBurstParticle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 velocity;
    private Vector3 startScale;
    private float lifetime;
    private float age;
    private float angularSpeed;
    private float gravity;

    public void Init(Vector2 startVelocity, float duration, float spin, float gravityStrength)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        velocity = startVelocity;
        startScale = transform.localScale;
        lifetime = duration;
        angularSpeed = spin;
        gravity = gravityStrength;
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.Rotate(0f, 0f, angularSpeed * Time.deltaTime);

        float t = age / lifetime;
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

        Color color = spriteRenderer.color;
        color.a = 1f - t;
        spriteRenderer.color = color;
    }
}
