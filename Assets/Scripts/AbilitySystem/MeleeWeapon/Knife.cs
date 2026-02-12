using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 1.5f;
    [SerializeField] private float hitRadius = 0.3f;

    private Vector2 dir;
    private float despawnTime;
    public void Init(Vector3 targetPos)
    {
        Vector2 d = targetPos - transform.position;
        dir = d.normalized;
        despawnTime = Time.time + lifeTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (Time.time >= despawnTime)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < EnemyRegistry.All.Count; i++)
        {
            var e = EnemyRegistry.All[i];
            if (e == null || e.IsDead) continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist > hitRadius + e.hitRadius) continue;

            e.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}