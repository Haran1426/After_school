using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private Slider Hpbar;
    public float power = 1f;

    private void Update()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (Hpbar != null)
            Hpbar.value = (float)currentHp;
    }
    protected override void Die()
    {
        GameRoot.Instance.Game.GameOver();
    }
}
