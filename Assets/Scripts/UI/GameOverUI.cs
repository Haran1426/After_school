using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;

    private PlayerExp playerExp;

    private void Awake()
    {
        CacheResultText();
        EnsureSummaryText();

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);

        if (titleButton != null)
            titleButton.onClick.AddListener(OnTitle);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        GameRoot.Instance.Game.OnGameOver += Show;
        GameRoot.Instance.Game.OnStageCleared += ShowStageClear;
        playerExp = FindAnyObjectByType<PlayerExp>();
    }

    private void OnDestroy()
    {
        if (GameRoot.Instance != null)
        {
            GameRoot.Instance.Game.OnGameOver -= Show;
            GameRoot.Instance.Game.OnStageCleared -= ShowStageClear;
        }
    }

    private void Show()
    {
        ShowResult("Game Over");
    }

    private void ShowStageClear()
    {
        ShowResult("스테이지 클리어");
    }

    private void ShowResult(string result)
    {
        gameObject.SetActive(true);

        if (resultText != null)
            resultText.text = result;

        float t = GameRoot.Instance.Game.SurvivedTime;
        int minutes = (int)(t / 60f);
        int seconds = (int)(t % 60f);

        if (timeText != null)
            timeText.text = $"생존 시간  {minutes:00}:{seconds:00}";

        if (killText != null)
            killText.text = $"처치 수  {GameRoot.Instance.Game.KillCount}";

        if (summaryText != null)
            summaryText.text = BuildSummary(result, t, GameRoot.Instance.Game.KillCount);
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;
        ScenesManager.Instance.ReloadCurrent();
    }

    private void OnTitle()
    {
        Time.timeScale = 1f;
        ScenesManager.Instance.Load(SceneId.TitleScene);
    }

    private void CacheResultText()
    {
        if (resultText != null)
            return;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            TextMeshProUGUI text = texts[i];
            if (text == null || text == timeText || text == killText)
                continue;

            resultText = text;
            return;
        }
    }

    private void EnsureSummaryText()
    {
        if (summaryText != null)
            return;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null && texts[i].name == "ResultSummaryText")
            {
                summaryText = texts[i];
                return;
            }
        }

        RectTransform parent = resultText != null && resultText.transform.parent is RectTransform resultParent
            ? resultParent
            : transform as RectTransform;

        if (parent == null)
            return;

        GameObject textObject = new GameObject("ResultSummaryText");
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -90f);
        rect.sizeDelta = new Vector2(520f, 86f);

        summaryText = textObject.AddComponent<TextMeshProUGUI>();
        summaryText.alignment = TextAlignmentOptions.Center;
        summaryText.fontSize = 24f;
        summaryText.color = new Color(0.96f, 0.94f, 0.84f, 1f);
        summaryText.textWrappingMode = TextWrappingModes.Normal;
        summaryText.text = string.Empty;
    }

    private string BuildSummary(string result, float survivedTime, int kills)
    {
        if (playerExp == null)
            playerExp = FindAnyObjectByType<PlayerExp>();

        int level = playerExp != null ? playerExp.level : 1;
        string grade = GetGrade(result, survivedTime, kills, level);
        return $"등급 {grade}\n레벨 {level}  /  처치 {kills}  /  생존 {FormatTime(survivedTime)}";
    }

    private string GetGrade(string result, float survivedTime, int kills, int level)
    {
        if (result.Contains("클리어"))
            return "S";

        float score = survivedTime * 0.08f + kills * 0.65f + level * 4f;
        if (score >= 160f) return "A";
        if (score >= 95f) return "B";
        if (score >= 45f) return "C";
        return "D";
    }

    private static string FormatTime(float seconds)
    {
        int clamped = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return $"{clamped / 60:00}:{clamped % 60:00}";
    }
}
