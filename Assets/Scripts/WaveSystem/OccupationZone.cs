using UnityEngine;
using UnityEngine.Events;

public enum OccupationRewardType
{
    LargeExp,
    UpgradeChoice,
    Heal,
    WeaponOrUpgrade,
    SpecialUpgrade
}

[RequireComponent(typeof(Collider2D))]
public sealed class OccupationZone : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float requiredStaySeconds = 10f;
    [SerializeField] private bool resetProgressOnExit = true;
    [SerializeField] private bool deactivateOnComplete = true;
    [SerializeField] private bool createFallbackVisual = true;
    [SerializeField] private Color fallbackColor = new Color(0.2f, 1f, 0.4f, 0.28f);

    [Header("Reward")]
    [SerializeField] private OccupationRewardType[] rewardPool = { OccupationRewardType.LargeExp, OccupationRewardType.UpgradeChoice, OccupationRewardType.Heal };
    [SerializeField, Min(0)] private int expReward = 20;
    [SerializeField, Min(0f)] private float healReward = 20f;

    public UnityEvent onCompleted;

    private bool playerInside;
    private bool completed;
    private float timer;
    private Player player;
    private PlayerExp playerExp;
    private LevelUpUI levelUpUI;
    private SpriteRenderer fallbackRenderer;
    private static Sprite fallbackSprite;

    public float Progress01 => requiredStaySeconds > 0f ? Mathf.Clamp01(timer / requiredStaySeconds) : 1f;
    public bool IsCompleted => completed;

    private void Awake()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        zoneCollider.isTrigger = true;

        EnsureFallbackVisual(zoneCollider);
    }

    private void OnEnable()
    {
        ResetZone();
    }

    private void Update()
    {
        if (completed || !playerInside || IsGameOver())
            return;

        timer += Time.deltaTime;
        UpdateFallbackVisual();

        if (timer >= requiredStaySeconds)
            Complete();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        CachePlayer(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (resetProgressOnExit && !completed)
        {
            timer = 0f;
            UpdateFallbackVisual();
        }
    }

    public void Configure(float staySeconds)
    {
        requiredStaySeconds = Mathf.Max(0.1f, staySeconds);
    }

    public void ResetZone()
    {
        completed = false;
        playerInside = false;
        timer = 0f;
        UpdateFallbackVisual();
    }

    private void Complete()
    {
        completed = true;
        timer = requiredStaySeconds;

        GrantReward();
        onCompleted?.Invoke();

        if (deactivateOnComplete)
            gameObject.SetActive(false);
    }

    private void GrantReward()
    {
        OccupationRewardType reward = PickReward();

        switch (reward)
        {
            case OccupationRewardType.Heal:
                HealPlayer();
                break;
            case OccupationRewardType.UpgradeChoice:
            case OccupationRewardType.WeaponOrUpgrade:
            case OccupationRewardType.SpecialUpgrade:
                ShowUpgradeChoice();
                break;
            default:
                GrantExp();
                break;
        }

        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.RewardSelect);
    }

    private OccupationRewardType PickReward()
    {
        if (rewardPool == null || rewardPool.Length == 0)
            return OccupationRewardType.LargeExp;

        return rewardPool[Random.Range(0, rewardPool.Length)];
    }

    private void GrantExp()
    {
        if (playerExp == null)
            playerExp = FindAnyObjectByType<PlayerExp>();

        if (playerExp != null)
            playerExp.AddExp(expReward);
    }

    private void HealPlayer()
    {
        if (player == null)
            player = FindAnyObjectByType<Player>();

        if (player == null)
        {
            GrantExp();
            return;
        }

        player.currentHp = Mathf.Min(player.maxHp, player.currentHp + healReward);
    }

    private void ShowUpgradeChoice()
    {
        if (levelUpUI == null)
            levelUpUI = FindLevelUpUI();

        if (levelUpUI != null)
            levelUpUI.Show();
        else
            GrantExp();
    }

    private void CachePlayer(GameObject playerObject)
    {
        if (player == null)
            player = playerObject.GetComponent<Player>();

        if (playerExp == null)
            playerExp = playerObject.GetComponent<PlayerExp>();
    }

    private LevelUpUI FindLevelUpUI()
    {
        LevelUpUI ui = FindAnyObjectByType<LevelUpUI>();
        if (ui != null)
            return ui;

        LevelUpUI[] all = Resources.FindObjectsOfTypeAll<LevelUpUI>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null && all[i].gameObject.scene.IsValid())
                return all[i];
        }

        return null;
    }

    private bool IsGameOver()
    {
        return GameRoot.Instance?.Game?.IsGameOver ?? false;
    }

    private void EnsureFallbackVisual(Collider2D zoneCollider)
    {
        if (!createFallbackVisual || GetComponentInChildren<SpriteRenderer>() != null)
            return;

        GameObject visual = new GameObject("OccupationZoneFallbackVisual");
        visual.transform.SetParent(transform, false);

        fallbackRenderer = visual.AddComponent<SpriteRenderer>();
        fallbackRenderer.sprite = GetFallbackSprite();
        fallbackRenderer.color = fallbackColor;
        fallbackRenderer.sortingOrder = 25;

        float radius = 2.5f;
        if (zoneCollider is CircleCollider2D circle)
            radius = circle.radius;

        visual.transform.localScale = Vector3.one * radius * 2f;
    }

    private void UpdateFallbackVisual()
    {
        if (fallbackRenderer == null)
            return;

        Color color = fallbackColor;
        color.a = Mathf.Lerp(fallbackColor.a, 0.55f, Progress01);
        fallbackRenderer.color = color;
    }

    private static Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null)
            return fallbackSprite;

        const int size = 64;
        const float center = (size - 1) * 0.5f;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "GeneratedOccupationZoneCircle";
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color fill = Color.white;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - center) / center;
                float dy = (y - center) / center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                texture.SetPixel(x, y, distance <= 1f ? fill : clear);
            }
        }

        texture.Apply();
        fallbackSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return fallbackSprite;
    }
}
