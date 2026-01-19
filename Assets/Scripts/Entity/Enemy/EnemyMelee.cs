using UnityEngine;

public class EnemyMelee : EnemyBase
{
    protected override void UpdateBehavior()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
