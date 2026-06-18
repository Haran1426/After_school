using UnityEngine;

public class CatClawAbility : MonoBehaviour
{
    [SerializeField] private float interval = 0.65f;
    [SerializeField] private float range = 2.4f;
    [SerializeField] private float hitRadius = 1.1f;
    [SerializeField] private float damage = 2f;

    private float nextTime;

    private void Update()
    {
        if (GameRoot.Instance?.Game?.IsGameOver == true)
            return;

        if (Time.time < nextTime)
            return;

        EnemyBase target = FindNearestEnemy();
        if (target == null)
            return;

        Vector3 hitCenter = target.transform.position;
        for (int i = EnemyRegistry.All.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = EnemyRegistry.All[i];
            if (enemy == null || enemy.IsDead)
                continue;

            float dist = Vector2.Distance(hitCenter, enemy.transform.position);
            if (dist <= hitRadius + enemy.hitRadius)
                enemy.TakeDamage(damage);
        }

        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.ClawSwipe);
        nextTime = Time.time + interval;
    }

    private EnemyBase FindNearestEnemy()
    {
        EnemyBase best = null;
        float bestDist = range;

        for (int i = EnemyRegistry.All.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = EnemyRegistry.All[i];
            if (enemy == null || enemy.IsDead)
                continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = enemy;
            }
        }

        return best;
    }
}
