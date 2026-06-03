using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerDangerHUD : MonoBehaviour
{
    [SerializeField, Range(0.05f, 0.9f)] private float dangerThreshold = 0.35f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.34f;
    [SerializeField, Min(0.1f)] private float pulseSpeed = 4.5f;

    private CanvasGroup canvasGroup;
    private Image overlay;
    private Player player;

    private void Awake()
    {
        BuildFallbackUI();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Hide();
    }

    private void Start()
    {
        CachePlayer();

        if (GameRoot.Instance != null)
        {
            GameRoot.Instance.Game.OnGameOver += Hide;
            GameRoot.Instance.Game.OnStageCleared += Hide;
        }
    }

    private void OnDestroy()
    {
        if (GameRoot.Instance != null)
        {
            GameRoot.Instance.Game.OnGameOver -= Hide;
            GameRoot.Instance.Game.OnStageCleared -= Hide;
        }
    }

    private void Update()
    {
        if (GameRoot.Instance == null || GameRoot.Instance.Game.IsGameOver || GameRoot.Instance.Game.IsStageCleared)
        {
            Hide();
            return;
        }

        if (player == null)
            CachePlayer();

        if (player == null || player.maxHp <= 0f)
        {
            Hide();
            return;
        }

        float hp01 = Mathf.Clamp01(player.currentHp / player.maxHp);
        if (hp01 > dangerThreshold)
        {
            Hide();
            return;
        }

        float severity = Mathf.InverseLerp(dangerThreshold, 0f, hp01);
        float pulse = 0.55f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.45f;
        Show(Mathf.Lerp(maxAlpha * 0.35f, maxAlpha, severity) * pulse);
    }

    public static void EnsureExists()
    {
        if (FindAnyObjectByType<PlayerDangerHUD>() != null)
            return;

        GameObject root = new GameObject("PlayerDangerHUD", typeof(RectTransform));
        root.AddComponent<PlayerDangerHUD>();
    }

    private void CachePlayer()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Show(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Clamp01(alpha);
    }

    private void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void BuildFallbackUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 29;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        RectTransform root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        GameObject imageObject = new GameObject("DangerOverlay");
        imageObject.transform.SetParent(transform, false);
        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        overlay = imageObject.AddComponent<Image>();
        overlay.raycastTarget = false;
        overlay.color = Color.white;
        overlay.sprite = CreateDangerSprite();
        overlay.type = Image.Type.Sliced;
    }

    private static Sprite CreateDangerSprite()
    {
        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float maxDistance = center.x;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float edgeDistance = Mathf.Min(Mathf.Min(x, size - 1 - x), Mathf.Min(y, size - 1 - y));
                float edgeAlpha = Mathf.SmoothStep(1f, 0f, edgeDistance / (size * 0.28f));
                float radial = Vector2.Distance(new Vector2(x, y), center) / maxDistance;
                float alpha = Mathf.Clamp01(edgeAlpha * Mathf.SmoothStep(0.2f, 1f, radial));
                texture.SetPixel(x, y, new Color(1f, 0.07f, 0.03f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect, new Vector4(24f, 24f, 24f, 24f));
    }
}
