using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class SurvivalTimerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private string label = "생존 시간";

    private int lastShownSecond = -1;

    private void Awake()
    {
        if (timeText == null)
            timeText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        lastShownSecond = -1;
        UpdateText(true);
    }

    private void Update()
    {
        UpdateText(false);
    }

    private void UpdateText(bool force)
    {
        if (timeText == null || GameRoot.Instance == null)
            return;

        int seconds = Mathf.Max(0, Mathf.FloorToInt(GameRoot.Instance.Game.SurvivedTime));
        if (!force && seconds == lastShownSecond)
            return;

        lastShownSecond = seconds;
        timeText.text = $"{label}  {seconds / 60:00}:{seconds % 60:00}";
    }
}
