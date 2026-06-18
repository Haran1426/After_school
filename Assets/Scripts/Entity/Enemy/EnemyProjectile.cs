using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 1f;
    public float lifeTime = 3f;
    public float hitRadius = 0.3f;

    Vector3 dir;
    float timer;
    Transform player;
    Entity playerEntity;

    public void Fire(Vector3 direction)
    {
        dir = direction.normalized;
        timer = 0f;
        CachePlayer();
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        CheckHitPlayer();
    }

    void CheckHitPlayer()
    {
        if (player == null)
            CachePlayer();

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > hitRadius)
            return;

        if (playerEntity != null)
            playerEntity.TakeDamage(damage);

        Destroy(gameObject);
    }

    private void CachePlayer()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target == null)
        {
            player = null;
            playerEntity = null;
            return;
        }

        player = target.transform;
        playerEntity = target.GetComponent<Entity>();
    }
}
