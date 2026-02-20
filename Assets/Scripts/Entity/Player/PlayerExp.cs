using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExp : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int requiredExp = 5;

    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        UpdateUI();
    }

    public void AddExp(int value)
    {
        currentExp += value;

        if (currentExp >= requiredExp)
            LevelUp();

        UpdateUI();
    }

    private void LevelUp()
    {
        currentExp -= requiredExp;
        level++;
        requiredExp = CalculateNextExp();
        gameObject.SetActive(true);
    }

    private int CalculateNextExp()
    {
        return level * 5;
    }

    private void UpdateUI()
    {
        if (expSlider != null)
            expSlider.value = (float)currentExp / requiredExp;

        if (levelText != null)
            levelText.text = "Lv" + level;
    }
}
