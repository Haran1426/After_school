using UnityEngine;

public abstract class EnemyBase : Entity
{
    public float moveSpeed = 2f;
    public int expReward = 1;

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
        GiveExp();
        gameObject.SetActive(false);
    }

    private void GiveExp()
    {
        PlayerExp playerExp = player.GetComponent<PlayerExp>();
        if (playerExp != null)
            playerExp.AddExp(expReward);
    }
}
