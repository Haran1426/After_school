using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float maxHp = 10f;
    public float currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();
    }

    protected abstract void Die();
}
