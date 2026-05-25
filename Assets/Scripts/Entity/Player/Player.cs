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
        {
            Hpbar.maxValue = maxHp;
            Hpbar.value = currentHp;
        }
    }

    public override void TakeDamage(float damage)
    {
        bool canTakeDamage = !IsDead && currentHp > 0f;
        base.TakeDamage(damage);

        if (canTakeDamage)
            GameRoot.Instance.Audio.PlaySfx(AudioCue.PlayerHit);
    }

    protected override void Die()
    {
        GameRoot.Instance.Game.GameOver();
    }
}
