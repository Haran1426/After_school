using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestart);
        titleButton.onClick.AddListener(OnTitle);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        GameRoot.Instance.Game.OnGameOver += Show;
    }

    private void OnDestroy()
    {
        if (GameRoot.Instance != null)
            GameRoot.Instance.Game.OnGameOver -= Show;
    }

    private void Show()
    {
        gameObject.SetActive(true);

        float t = GameRoot.Instance.Game.SurvivedTime;
        int minutes = (int)(t / 60f);
        int seconds = (int)(t % 60f);
        timeText.text = $"생존 시간  {minutes:00}:{seconds:00}";

        killText.text = $"처치 수  {GameRoot.Instance.Game.KillCount}";
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
}
