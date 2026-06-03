using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BossHealthHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;
    [SerializeField] private CanvasGroup canvasGroup;

    private BossEnemy trackedBoss;

    private void Awake()
    {
        if (bossNameText == null || healthFill == null)
            BuildFallbackUI();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Hide();
    }

    private void Start()
    {
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
        trackedBoss = BossEnemy.ActiveBoss;

        if (trackedBoss == null || !trackedBoss.IsActiveBoss)
        {
            Hide();
            return;
        }

        Show();

        if (bossNameText != null)
            bossNameText.text = "거대 멧돼지";

        if (healthFill != null)
            healthFill.fillAmount = trackedBoss.Health01;
    }

    public static void EnsureExists()
    {
        if (FindAnyObjectByType<BossHealthHUD>() != null)
            return;

        GameObject root = new GameObject("BossHealthHUD", typeof(RectTransform));
        root.AddComponent<BossHealthHUD>();
    }

    private void Show()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 1f;
    }

    private void Hide()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
    }

    private void BuildFallbackUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 41;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        GameObject panel = new GameObject("BossHealthPanel");
        panel.transform.SetParent(transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -118f);
        panelRect.sizeDelta = new Vector2(560f, 48f);

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.13f, 0.04f, 0.03f, 0.72f);

        bossNameText = CreateText(panelRect, "BossNameText", new Vector2(0f, -13f), 18f, new Color(1f, 0.75f, 0.58f, 1f));

        GameObject bar = new GameObject("BossHealthBar");
        bar.transform.SetParent(panelRect, false);
        RectTransform barRect = bar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0f);
        barRect.anchorMax = new Vector2(0.5f, 0f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = new Vector2(0f, 12f);
        barRect.sizeDelta = new Vector2(512f, 12f);

        Image barBackground = bar.AddComponent<Image>();
        barBackground.color = new Color(1f, 1f, 1f, 0.16f);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barRect, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        healthFill = fill.AddComponent<Image>();
        healthFill.color = new Color(0.95f, 0.16f, 0.08f, 0.95f);
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
    }

    private TextMeshProUGUI CreateText(RectTransform parent, string objectName, Vector2 anchoredPosition, float fontSize, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(520f, 24f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.text = string.Empty;
        return text;
    }
}
