using UnityEngine;

public class EnemyRanged : EnemyBase
{
    public float attackRange = 5f;
    public float fireInterval = 1.5f;

    public EnemyProjectile projectilePrefab;
    public Transform firePoint;

    float fireTimer;

    protected override void UpdateBehavior()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                fireTimer = 0f;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        EnemyProjectile proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        proj.Fire(dir);
    }
}
