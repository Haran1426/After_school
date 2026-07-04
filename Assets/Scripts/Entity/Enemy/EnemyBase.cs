using System.Collections.Generic;
using UnityEngine;

public static class EnemyRegistry
{
    public static readonly List<EnemyBase> All = new();
}
public abstract class EnemyBase : Entity
{
    public float moveSpeed = 2f;
    public float hitRadius = 0.5f;

    [SerializeField] private PoolType expOrbType = PoolType.ExpOrb;
    [SerializeField] private int expReward = 1;
    [SerializeField, Min(0f)] private float hitStunSeconds = 0.12f;
    [SerializeField, Min(0f)] private float knockbackSeconds = 0.1f;
    [SerializeField, Min(0f)] private float knockbackSpeed = 7.5f;
    [SerializeField, Min(0f)] private float contactDamage = 1f;
    [SerializeField, Min(0.05f)] private float contactDamageInterval = 0.65f;

    protected Transform player;
    Player playerEntity;
    public event System.Action<EnemyBase> Died;

    bool isStunned;
    float stunEndTime;

    Vector3 knockbackDir;
    float knockbackEndTime;
    bool isKnockback;
    float nextContactDamageTime;

    protected override void Awake()
    {
        base.Awake();
        RegisterEnemy();
    }

    protected virtual void Start()
    {
        CachePlayer();
    }

    protected virtual void Update()
    {
        if (IsDead)
            return;

        if (player == null)
            CachePlayer();

        if (player == null)
            return;

        if (isKnockback)
        {
            if (Time.time < knockbackEndTime)
            {
                transform.position += knockbackDir * knockbackSpeed * Time.deltaTime;
                return;
            }

            isKnockback = false;
        }

        if (isStunned)
        {
            if (Time.time >= stunEndTime) isStunned = false;
            else return;
        }

        UpdateBehavior();
        TryDealContactDamage();
    }
    public override void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        Vector3 hitDir = player != null
            ? (transform.position - player.position).normalized
            : (Vector3)Random.insideUnitCircle.normalized;

        if (hitDir.sqrMagnitude <= 0.0001f)
            hitDir = Random.insideUnitCircle.normalized;

        currentHp -= damage;

        DamageTextSpawner.Instance?.Spawn(damage, transform.position);
        GameJuiceFX.HitSpark(transform.position, hitDir, new Color(1f, 0.88f, 0.28f, 0.95f), currentHp <= 0f ? 14 : 7);
        GameJuiceFX.Shake(currentHp <= 0f ? 0.12f : 0.055f, currentHp <= 0f ? 0.08f : 0.035f);

        if (currentHp <= 0f)
        {
            Die();
            return;
        }

        isStunned = true;
        GameRoot.Instance.Audio.PlaySfx(AudioCue.EnemyHit);
        stunEndTime = Time.time + hitStunSeconds;

        knockbackDir = hitDir;
        knockbackEndTime = Time.time + knockbackSeconds;
        isKnockback = true;
    }
    protected override void Die()
    {
        if (IsDead) return;
        IsDead = true;

        GameRoot.Instance.Audio.PlaySfx(AudioCue.EnemyDie);
        DropExp();
        GameRoot.Instance.Game.RegisterKill();
        Died?.Invoke(this);

        UnregisterEnemy();
        Blade.ClearHitCache(this);

        var po = GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else gameObject.SetActive(false);
    }
    private void DropExp()
    {
        var orbObj = GameRoot.Instance.Pool.Spawn(expOrbType, transform.position, Quaternion.identity);

        if (orbObj == null) return;

        var orb = orbObj.GetComponent<ExpOrb>();
        if (orb != null)
            orb.expValue = expReward;
    }
    public override void OnSpawned()
    {
        base.OnSpawned();

        isStunned = false;
        stunEndTime = 0f;

        isKnockback = false;
        knockbackEndTime = 0f;
        nextContactDamageTime = 0f;

        RegisterEnemy();
        Blade.ClearHitCache(this);
        currentHp = maxHp;
        IsDead = false;
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        UnregisterEnemy();
    }

    private void RegisterEnemy()
    {
        if (!EnemyRegistry.All.Contains(this))
            EnemyRegistry.All.Add(this);
    }

    private void UnregisterEnemy()
    {
        EnemyRegistry.All.Remove(this);
    }

    private void CachePlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            player = null;
            playerEntity = null;
            return;
        }

        player = p.transform;
        playerEntity = p.GetComponent<Player>();
    }

    private void TryDealContactDamage()
    {
        if (contactDamage <= 0f || playerEntity == null || Time.time < nextContactDamageTime)
            return;

        float contactRadius = Mathf.Max(0.05f, hitRadius);
        if ((player.position - transform.position).sqrMagnitude > contactRadius * contactRadius)
            return;

        nextContactDamageTime = Time.time + contactDamageInterval;
        playerEntity.TakeDamage(contactDamage);
    }

    protected abstract void UpdateBehavior();
}
