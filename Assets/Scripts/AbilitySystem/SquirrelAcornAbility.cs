using UnityEngine;

public class SquirrelAcornAbility : MonoBehaviour
{
    [SerializeField] private float interval = 0.55f;
    [SerializeField] private float range = 7f;
    [SerializeField] private float damage = 1.4f;

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

        target.TakeDamage(damage);
        GameRoot.Instance?.Audio?.PlaySfx(AudioCue.AcornShot);
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
