using UnityEngine;

public class BunnyShockwaveAbility : MonoBehaviour
{
    [SerializeField] private float interval = 1.35f;
    [SerializeField] private float radius = 2.1f;
    [SerializeField] private float damage = 1.6f;

    private float nextTime;

    private void Update()
    {
        if (GameRoot.Instance?.Game?.IsGameOver == true)
            return;

        if (Time.time < nextTime)
            return;

        bool hitAny = false;
        for (int i = EnemyRegistry.All.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = EnemyRegistry.All[i];
            if (enemy == null || enemy.IsDead)
                continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist > radius + enemy.hitRadius)
                continue;

            enemy.TakeDamage(damage);
            hitAny = true;
        }

        if (hitAny)
            GameRoot.Instance?.Audio?.PlaySfx(AudioCue.LeafBurst);

        nextTime = Time.time + interval;
    }
}
