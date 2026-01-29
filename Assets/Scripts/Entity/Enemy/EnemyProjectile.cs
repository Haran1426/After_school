using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 1f;
    public float lifeTime = 3f;
    public float hitRadius = 0.3f;

    Vector3 dir;
    float timer;

    public void Fire(Vector3 direction)
    {
        dir = direction.normalized;
        timer = 0f;
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
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist > hitRadius)
            return;

        var entity = player.GetComponent<Entity>();
        if (entity != null)
            entity.TakeDamage(damage);

        Destroy(gameObject);
    }
}
