using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class StageProgressHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI nextWaveText;
    [SerializeField] private TextMeshProUGUI eliteText;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Image progressFill;

    private WaveManager waveManager;
    private CanvasGroup canvasGroup;
    private float alertTimer;

    private void Awake()
    {
        if (stageText == null || timeText == null || nextWaveText == null || eliteText == null || alertText == null)
            BuildFallbackUI();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        BindWaveManager();

        if (GameRoot.Instance != null)
        {
            GameRoot.Instance.Game.OnGameOver += Hide;
            GameRoot.Instance.Game.OnStageCleared += Hide;
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnPhaseChanged -= HandlePhaseChanged;
            waveManager.OnBossWarning -= HandleBossWarning;
            waveManager.OnBossTimeReached -= HandleBossTimeReached;
        }

        if (GameRoot.Instance != null)
        {
            GameRoot.Instance.Game.OnGameOver -= Hide;
            GameRoot.Instance.Game.OnStageCleared -= Hide;
        }
    }

    private void Update()
    {
        if (GameRoot.Instance == null)
            return;

        if (waveManager == null)
            BindWaveManager();

        if (!ShouldShow())
        {
            Hide();
            return;
        }

        Show();
        UpdateTexts();
        UpdateAlert();
    }

    public static void EnsureExists()
    {
        if (FindAnyObjectByType<StageProgressHUD>() != null)
            return;

        GameObject root = new GameObject("StageProgressHUD", typeof(RectTransform));
        root.AddComponent<StageProgressHUD>();
    }

    private void BindWaveManager()
    {
        WaveManager next = WaveManager.Instance != null ? WaveManager.Instance : FindAnyObjectByType<WaveManager>();
        if (next == waveManager)
            return;

        if (waveManager != null)
        {
            waveManager.OnPhaseChanged -= HandlePhaseChanged;
            waveManager.OnBossWarning -= HandleBossWarning;
            waveManager.OnBossTimeReached -= HandleBossTimeReached;
        }

        waveManager = next;

        if (waveManager != null)
        {
            waveManager.OnPhaseChanged += HandlePhaseChanged;
            waveManager.OnBossWarning += HandleBossWarning;
            waveManager.OnBossTimeReached += HandleBossTimeReached;
        }
    }

    private void UpdateTexts()
    {
        GameManager game = GameRoot.Instance.Game;
        StageMap stage = waveManager != null ? waveManager.Stage : null;
        float survived = game.SurvivedTime;
        float duration = stage != null ? stage.DurationSeconds : 600f;

        if (stageText != null)
            stageText.text = stage != null ? stage.StageName : "야생의 숲";

        if (timeText != null)
            timeText.text = $"{FormatTime(survived)} / {FormatTime(duration)}";

        if (progressFill != null)
            progressFill.fillAmount = duration > 0f ? Mathf.Clamp01(survived / duration) : 0f;

        if (nextWaveText != null)
        {
            float nextPhase = waveManager != null ? waveManager.TimeUntilNextPhase() : -1f;
            nextWaveText.text = nextPhase >= 0f
                ? $"다음 웨이브 {FormatTime(nextPhase)}"
                : $"보스까지 {FormatTime(Mathf.Max(0f, duration - survived))}";
        }

        if (eliteText != null)
            eliteText.text = BuildEliteText(stage, survived);
    }

    private string BuildEliteText(StageMap stage, float survived)
    {
        if (stage == null || stage.EliteSpawnTimes == null || stage.EliteSpawnTimes.Length == 0)
            return "정예 대기";

        for (int i = 0; i < stage.EliteSpawnTimes.Length; i++)
        {
            float remaining = stage.EliteSpawnTimes[i] - survived;
            if (remaining > 0f)
                return $"정예 {FormatTime(remaining)}";
        }

        return "정예 출현 완료";
    }

    private void UpdateAlert()
    {
        if (alertText == null)
            return;

        alertTimer = Mathf.Max(0f, alertTimer - Time.unscaledDeltaTime);
        Color color = alertText.color;
        color.a = alertTimer > 0f ? Mathf.Clamp01(alertTimer / 0.35f) : 0f;
        alertText.color = color;
    }

    private void HandlePhaseChanged(int phaseIndex)
    {
        WavePhaseData phase = waveManager != null ? waveManager.CurrentPhase : null;
        ShowAlert(phase != null ? phase.label : "새 웨이브");
    }

    private void HandleBossTimeReached()
    {
        ShowAlert("보스 등장!", 3.2f);
    }

    private void HandleBossWarning()
    {
        ShowAlert("숲이 흔들립니다... 보스 접근!", 4f);
    }

    private void ShowAlert(string message, float duration = 2.5f)
    {
        if (alertText == null)
            return;

        alertText.text = message;
        alertTimer = Mathf.Max(0.1f, duration);
    }

    private bool ShouldShow()
    {
        return waveManager != null
            && GameRoot.Instance != null
            && !GameRoot.Instance.Game.IsGameOver
            && !GameRoot.Instance.Game.IsStageCleared;
    }

    private void Show()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
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
        canvas.sortingOrder = 40;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        GameObject panel = new GameObject("StageProgressPanel");
        panel.transform.SetParent(transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -18f);
        panelRect.sizeDelta = new Vector2(540f, 86f);

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.06f, 0.08f, 0.05f, 0.62f);

        stageText = CreateText(panelRect, "StageText", new Vector2(0f, -14f), 24f, TextAlignmentOptions.Center, new Color(1f, 0.94f, 0.72f, 1f));
        timeText = CreateText(panelRect, "TimeText", new Vector2(0f, -42f), 22f, TextAlignmentOptions.Center, Color.white);
        nextWaveText = CreateText(panelRect, "NextWaveText", new Vector2(-128f, -66f), 16f, TextAlignmentOptions.Center, new Color(0.82f, 1f, 0.76f, 1f));
        eliteText = CreateText(panelRect, "EliteText", new Vector2(128f, -66f), 16f, TextAlignmentOptions.Center, new Color(1f, 0.78f, 0.4f, 1f));

        GameObject bar = new GameObject("StageProgressBar");
        bar.transform.SetParent(panelRect, false);
        RectTransform barRect = bar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0f);
        barRect.anchorMax = new Vector2(0.5f, 0f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = new Vector2(0f, 6f);
        barRect.sizeDelta = new Vector2(500f, 8f);
        Image barBackground = bar.AddComponent<Image>();
        barBackground.color = new Color(1f, 1f, 1f, 0.18f);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barRect, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        progressFill = fill.AddComponent<Image>();
        progressFill.color = new Color(0.48f, 0.95f, 0.34f, 0.9f);
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;

        alertText = CreateText((RectTransform)transform, "StageAlertText", new Vector2(0f, -118f), 34f, TextAlignmentOptions.Center, new Color(1f, 0.35f, 0.18f, 0f));
    }

    private TextMeshProUGUI CreateText(RectTransform parent, string objectName, Vector2 anchoredPosition, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(520f, 30f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.text = string.Empty;
        return text;
    }

    private static string FormatTime(float seconds)
    {
        int clamped = Mathf.Max(0, Mathf.CeilToInt(seconds));
        return $"{clamped / 60:00}:{clamped % 60:00}";
    }
}
