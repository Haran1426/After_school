using System.Collections.Generic;
using UnityEngine;

public sealed class ForestMapDecorator : MonoBehaviour
{
    [SerializeField] private StageMap stageMap;
    [SerializeField] private int seed = 20260608;
    [SerializeField, Min(0)] private int grassCount = 90;
    [SerializeField, Min(0)] private int bushCount = 30;
    [SerializeField, Min(0)] private int stoneCount = 18;
    [SerializeField, Min(0)] private int shadowTreeCount = 24;
    [SerializeField, Min(0f)] private float centerSafeRadius = 3.5f;
    [SerializeField] private bool generateOnAwake = true;

    private readonly List<GameObject> spawnedDecorations = new();
    private static Sprite circleSprite;
    private static Sprite bladeSprite;

    private void Awake()
    {
        if (generateOnAwake)
            Generate();
    }

    public void Generate()
    {
        ClearGenerated();

        if (stageMap == null)
            stageMap = GetComponent<StageMap>();

        Vector2 min = stageMap != null && stageMap.HasValidBounds() ? stageMap.BoundsMin : new Vector2(-22f, -14f);
        Vector2 max = stageMap != null && stageMap.HasValidBounds() ? stageMap.BoundsMax : new Vector2(22f, 14f);

        Random.State previousState = Random.state;
        Random.InitState(seed);

        SpawnGroup("Grass", grassCount, min, max, centerSafeRadius, CreateGrass);
        SpawnGroup("Bush", bushCount, min, max, centerSafeRadius + 0.5f, CreateBush);
        SpawnGroup("Stone", stoneCount, min, max, centerSafeRadius + 0.8f, CreateStone);
        SpawnGroup("TreeShadow", shadowTreeCount, min, max, centerSafeRadius + 1.5f, CreateTreeShadow);

        Random.state = previousState;
    }

    public void ClearGenerated()
    {
        for (int i = spawnedDecorations.Count - 1; i >= 0; i--)
        {
            if (spawnedDecorations[i] != null)
                Destroy(spawnedDecorations[i]);
        }

        spawnedDecorations.Clear();
    }

    private void SpawnGroup(string groupName, int count, Vector2 min, Vector2 max, float safeRadius, System.Action<Transform, Vector2> create)
    {
        GameObject group = new GameObject(groupName);
        group.transform.SetParent(transform, false);
        spawnedDecorations.Add(group);

        for (int i = 0; i < count; i++)
        {
            Vector2 position = PickPosition(min, max, safeRadius);
            create(group.transform, position);
        }
    }

    private Vector2 PickPosition(Vector2 min, Vector2 max, float safeRadius)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 position = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            if (position.sqrMagnitude >= safeRadius * safeRadius)
                return position;
        }

        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    private void CreateGrass(Transform parent, Vector2 position)
    {
        SpriteRenderer renderer = CreateDecoration(parent, "Grass", position, GetBladeSprite(), Random.Range(-75, -65));
        renderer.color = Random.ColorHSV(0.22f, 0.34f, 0.45f, 0.75f, 0.45f, 0.85f, 0.65f, 0.9f);
        renderer.transform.localScale = new Vector3(Random.Range(0.35f, 0.65f), Random.Range(0.45f, 0.9f), 1f);
        renderer.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f));
    }

    private void CreateBush(Transform parent, Vector2 position)
    {
        SpriteRenderer renderer = CreateDecoration(parent, "Bush", position, GetCircleSprite(), Random.Range(-72, -62));
        renderer.color = Random.ColorHSV(0.24f, 0.38f, 0.5f, 0.8f, 0.35f, 0.65f, 0.75f, 0.95f);
        float width = Random.Range(0.9f, 1.8f);
        renderer.transform.localScale = new Vector3(width, Random.Range(0.45f, 0.85f), 1f);
    }

    private void CreateStone(Transform parent, Vector2 position)
    {
        SpriteRenderer renderer = CreateDecoration(parent, "Stone", position, GetCircleSprite(), Random.Range(-70, -60));
        float value = Random.Range(0.42f, 0.62f);
        renderer.color = new Color(value, value + 0.04f, value + 0.02f, Random.Range(0.75f, 0.95f));
        renderer.transform.localScale = new Vector3(Random.Range(0.35f, 0.8f), Random.Range(0.22f, 0.45f), 1f);
        renderer.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-15f, 15f));
    }

    private void CreateTreeShadow(Transform parent, Vector2 position)
    {
        SpriteRenderer renderer = CreateDecoration(parent, "TreeShadow", position, GetCircleSprite(), Random.Range(-90, -82));
        renderer.color = new Color(0.12f, 0.22f, 0.1f, Random.Range(0.16f, 0.28f));
        renderer.transform.localScale = new Vector3(Random.Range(2.2f, 4.4f), Random.Range(1.4f, 2.7f), 1f);
        renderer.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-20f, 20f));
    }

    private SpriteRenderer CreateDecoration(Transform parent, string objectName, Vector2 position, Sprite sprite, int sortingOrder)
    {
        GameObject decoration = new GameObject(objectName);
        decoration.transform.SetParent(parent, false);
        decoration.transform.localPosition = new Vector3(position.x, position.y, 0f);

        SpriteRenderer renderer = decoration.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private static Sprite GetCircleSprite()
    {
        if (circleSprite != null)
            return circleSprite;

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "GeneratedForestCircle";
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(1f, 1f, 1f, 0f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - 31.5f) / 31.5f;
                float dy = (y - 31.5f) / 31.5f;
                texture.SetPixel(x, y, dx * dx + dy * dy <= 1f ? Color.white : clear);
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }

    private static Sprite GetBladeSprite()
    {
        if (bladeSprite != null)
            return bladeSprite;

        const int width = 16;
        const int height = 48;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.name = "GeneratedForestGrassBlade";
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(1f, 1f, 1f, 0f);
        for (int y = 0; y < height; y++)
        {
            float halfWidth = Mathf.Lerp(1.2f, 6f, y / (float)(height - 1));
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Abs(x - 7.5f);
                texture.SetPixel(x, y, dx <= halfWidth ? Color.white : clear);
            }
        }

        texture.Apply();
        bladeSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0f), height);
        return bladeSprite;
    }
}
