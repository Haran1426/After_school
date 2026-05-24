using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    protected Transform player;

    bool isStunned;
    float stunEndTime;

    Vector3 knockbackDir;
    float knockbackEndTime;
    bool isKnockback;

    protected override void Awake()
    {
        base.Awake();
        RegisterEnemy();
    }

    protected virtual void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    protected virtual void Update()
    {
        if (IsDead || player == null)
            return;

        if (isStunned)
        {
            if (Time.time >= stunEndTime) isStunned = false;
            else return;
        }

        UpdateBehavior();
    }
    public override void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        currentHp -= damage;

        DamageTextSpawner.Instance.Spawn(damage, transform.position);

        if (currentHp <= 0f)
        {
            Die();
            return;
        }

        isStunned = true;
        stunEndTime = Time.time + 0.15f;

        Vector3 dir = (transform.position - player.position).normalized;
        knockbackDir = dir;
        knockbackEndTime = Time.time + 0.1f;
        isKnockback = true;
    }
    protected override void Die()
    {
        if (IsDead) return;
        IsDead = true;

        DropExp();
        GameRoot.Instance.Game.RegisterKill();

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

    protected abstract void UpdateBehavior();
}
