using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button button;

    private RewardData currentData;

    public void Setup(RewardData data, System.Action<RewardData> onClick)
    {
        if (data == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        currentData = data;

        if (background != null && data.background != null)
            background.sprite = data.background;

        if (icon != null)
        {
            icon.sprite = data.icon;
            icon.preserveAspect = true;
            icon.color = Color.white;
            icon.gameObject.SetActive(data.icon != null);
        }

        if (title != null)
        {
            PrepareText(title, 26f, 36f, TextAlignmentOptions.Center);
            title.text = data.rewardName;
        }

        if (description != null)
        {
            PrepareText(description, 20f, 30f, TextAlignmentOptions.Center);
            description.text = FormatDescription(data.description);
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (currentData != null)
                    onClick?.Invoke(currentData);
            });
        }
    }

    public void Hide()
    {
        currentData = null;

        if (button != null)
            button.onClick.RemoveAllListeners();

        gameObject.SetActive(false);
    }

    private static void PrepareText(TextMeshProUGUI text, float minSize, float maxSize, TextAlignmentOptions alignment)
    {
        text.enableAutoSizing = true;
        text.fontSizeMin = minSize;
        text.fontSizeMax = maxSize;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.wordWrappingRatios = 0.35f;
        text.characterSpacing = 0f;
        text.lineSpacing = -8f;
    }

    private static string FormatDescription(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        return raw.Trim().Replace("경험치 구슬 흡수 범위", "경험치 흡수 범위");
    }
}
