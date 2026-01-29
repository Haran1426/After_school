using UnityEngine;
using System.Collections.Generic;

public class Blade : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    [SerializeField] float hitRadius = 0.8f;
    [SerializeField] float hitCooldown = 0.25f;

    static readonly Dictionary<EnemyBase, float> hitTimer = new();

    public static void ClearHitCache(EnemyBase enemy)
    {
        hitTimer.Remove(enemy);
    }

    void Update()
    {
        float time = Time.time;

        for (int i = EnemyRegistry.All.Count - 1; i >= 0; i--)
        {
            var enemy = EnemyRegistry.All[i];
            if (enemy == null || enemy.IsDead)
                continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);

            if (dist > hitRadius + enemy.hitRadius)
                continue;

            if (hitTimer.TryGetValue(enemy, out float last) &&
                time - last < hitCooldown)
                continue;

            enemy.TakeDamage(damage);
            hitTimer[enemy] = time;
        }
    }
}
