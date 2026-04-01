using TMPro;
using UnityEngine;

public class KillCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText;

    private void Start()
    {
        GameRoot.Instance.Game.OnGameOver += Hide;
        UpdateText();
    }

    private void OnDestroy()
    {
        if (GameRoot.Instance != null)
            GameRoot.Instance.Game.OnGameOver -= Hide;
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        killText.text = $"킬  {GameRoot.Instance.Game.KillCount}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
