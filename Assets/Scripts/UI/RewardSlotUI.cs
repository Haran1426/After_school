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

        if (background != null)
            background.sprite = data.background;

        if (icon != null)
        {
            icon.sprite = data.icon;
            icon.gameObject.SetActive(data.icon != null);
        }

        if (title != null)
            title.text = data.rewardName;

        if (description != null)
            description.text = data.description;

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
}
