using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 5f;

    Vector3 direction;

    public void Fire(Vector3 dir)
    {
        direction = dir.normalized;
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
