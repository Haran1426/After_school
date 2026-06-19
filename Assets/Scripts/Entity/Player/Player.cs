using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] private Slider Hpbar;
    [SerializeField, Min(0f)] private float invulnerableSeconds = 0.65f;
    [SerializeField, Min(1f)] private float hitFlashFps = 14f;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.62f, 0.62f, 1f);

    public float power = 1f;
    public float ExpMagnetBonus { get; private set; }

    private SpriteRenderer[] renderers;
    private Color[] baseColors;
    private float invulnerableUntil;
    private float dodgeChance;
    private float displayedHp;
    private bool hpDisplayInitialized;

    protected override void Awake()
    {
        base.Awake();
        CacheRenderers();
    }

    private void Update()
    {
        UpdateUI();
        UpdateHitFlash();
    }

    private void UpdateUI()
    {
        if (Hpbar != null)
        {
            Hpbar.maxValue = maxHp;

            if (!hpDisplayInitialized)
            {
                displayedHp = currentHp;
                hpDisplayInitialized = true;
            }

            float speed = currentHp < displayedHp ? maxHp * 1.8f : maxHp * 5f;
            displayedHp = Mathf.MoveTowards(displayedHp, currentHp, speed * Time.deltaTime);
            Hpbar.value = displayedHp;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead || currentHp <= 0f || Time.time < invulnerableUntil)
            return;

        if (dodgeChance > 0f && Random.value < dodgeChance)
        {
            invulnerableUntil = Time.time + invulnerableSeconds * 0.45f;
            DamageTextSpawner.Instance?.SpawnText("회피", transform.position + Vector3.up * 0.45f, new Color(0.72f, 1f, 0.55f, 1f));
            GameJuiceFX.HitSpark(transform.position, Vector3.up, new Color(0.56f, 1f, 0.62f, 0.9f), 5);
            GameRoot.Instance?.Audio?.PlaySfx(AudioCue.ExpPickup);
            return;
        }

        base.TakeDamage(damage);

        invulnerableUntil = Time.time + invulnerableSeconds;
        GameJuiceFX.HitSpark(transform.position, Vector3.up, new Color(1f, 0.25f, 0.18f, 0.88f), 9);
        GameJuiceFX.Shake(0.14f, 0.09f);
        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.PlayerHit);
    }

    public void AddExpMagnetRange(float value)
    {
        ExpMagnetBonus = Mathf.Max(0f, ExpMagnetBonus + value);
    }

    public void ConfigureCharacterPassive(PlayerCharacterType character)
    {
        dodgeChance = 0f;
        ExpMagnetBonus = 0f;

        switch (character)
        {
            case PlayerCharacterType.Cat:
                dodgeChance = 0.1f;
                break;
            case PlayerCharacterType.Bunny:
                dodgeChance = 0.22f;
                break;
            case PlayerCharacterType.Squirrel:
                ExpMagnetBonus = 0.9f;
                break;
        }
    }

    protected override void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        RestoreRendererColors();
        GameRoot.Instance.Game.GameOver();
    }

    private void CacheRenderers()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        baseColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            baseColors[i] = renderers[i].color;
    }

    private void UpdateHitFlash()
    {
        if (renderers == null || renderers.Length == 0)
            return;

        if (Time.time >= invulnerableUntil || IsDead)
        {
            RestoreRendererColors();
            return;
        }

        bool showFlash = Mathf.FloorToInt(Time.time * hitFlashFps) % 2 == 0;
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].color = showFlash ? hitFlashColor : baseColors[i];
    }

    private void RestoreRendererColors()
    {
        if (renderers == null || baseColors == null)
            return;

        for (int i = 0; i < renderers.Length && i < baseColors.Length; i++)
            renderers[i].color = baseColors[i];
    }
}
