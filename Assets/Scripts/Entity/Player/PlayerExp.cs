using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExp : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int requiredExp = 5;

    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private LevelUpUI levelUpUI;

    private void Awake()
    {
        ConfigureExpBar();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddExp(int value)
    {
        currentExp += value;

        while (currentExp >= requiredExp)
            LevelUp();

        UpdateUI();
    }

    private void LevelUp()
    {
        currentExp -= requiredExp;
        level++;
        requiredExp = CalculateNextExp();
        levelUpUI?.Show();
    }

    private int CalculateNextExp()
    {
        return level * 5;
    }

    private void UpdateUI()
    {
        if (expBar != null)
        {
            ConfigureExpBar();
            expBar.fillAmount = requiredExp > 0
                ? Mathf.Clamp01((float)currentExp / requiredExp)
                : 0f;
        }

        if (levelText != null)
            levelText.text = "Lv" + level;
    }

    private void ConfigureExpBar()
    {
        if (expBar == null) return;

        expBar.type = Image.Type.Filled;
        expBar.fillMethod = Image.FillMethod.Horizontal;
        expBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }
}
