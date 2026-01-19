using UnityEngine;

public abstract class EnemyBase : Entity
{
    public float moveSpeed = 2f;
    protected Transform player;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        UpdateBehavior();
    }

    protected abstract void UpdateBehavior();

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
