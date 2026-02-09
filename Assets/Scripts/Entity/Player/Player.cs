using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private Slider Hpbar;

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
        //GameManager.Instance.GameOver();
    }
}
