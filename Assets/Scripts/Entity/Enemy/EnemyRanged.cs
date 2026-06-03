using UnityEngine;

public class EnemyRanged : EnemyBase
{
    public float attackRange = 5f;
    public float fireInterval = 1.5f;
    [SerializeField, Min(0.1f)] private float preferredRange = 4.2f;
    [SerializeField, Min(0.1f)] private float retreatRange = 2.6f;
    [SerializeField, Range(0f, 1f)] private float strafeStrength = 0.45f;
    [SerializeField, Min(0.1f)] private float strafeChangeInterval = 1.2f;

    public EnemyProjectile projectilePrefab;
    public Transform firePoint;

    float fireTimer;
    float strafeDirection = 1f;
    float nextStrafeChangeTime;

    public override void OnSpawned()
    {
        base.OnSpawned();
        fireTimer = Random.Range(0f, fireInterval * 0.35f);
        PickStrafeDirection();
    }

    protected override void UpdateBehavior()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        Vector3 awayFromPlayer = (transform.position - player.position).normalized;
        if (awayFromPlayer.sqrMagnitude <= 0.0001f)
            awayFromPlayer = Vector3.right;

        if (Time.time >= nextStrafeChangeTime)
            PickStrafeDirection();

        if (dist > attackRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            MoveAtCombatRange(dist, awayFromPlayer);
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

    private void MoveAtCombatRange(float dist, Vector3 awayFromPlayer)
    {
        Vector3 tangent = new Vector3(-awayFromPlayer.y, awayFromPlayer.x, 0f) * strafeDirection;
        Vector3 moveDir = tangent * strafeStrength;

        if (dist < retreatRange)
            moveDir += awayFromPlayer;
        else if (dist < preferredRange)
            moveDir += awayFromPlayer * 0.35f;

        if (moveDir.sqrMagnitude <= 0.001f)
            return;

        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    private void PickStrafeDirection()
    {
        strafeDirection = Random.value < 0.5f ? -1f : 1f;
        nextStrafeChangeTime = Time.time + Random.Range(strafeChangeInterval * 0.7f, strafeChangeInterval * 1.3f);
    }
}
