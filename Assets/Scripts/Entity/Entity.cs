using UnityEngine;

public abstract class Entity : MonoBehaviour, IPoolable
{
    public float maxHp = 10f;
    public float currentHp;

    public bool IsDead { get; protected set; }

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void OnSpawned()
    {
        currentHp = maxHp;
        IsDead = false;
    }

    public virtual void OnDespawned()
    {

    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead || currentHp <= 0f)
            return;

        currentHp -= damage;

        if (currentHp <= 0f)
            Die();
    }

    protected abstract void Die();
}
