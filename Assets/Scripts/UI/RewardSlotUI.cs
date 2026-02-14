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
        currentData = data;

        background.sprite = data.background;
        icon.sprite = data.icon;
        title.text = data.rewardName;
        description.text = data.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(currentData));
    }
}