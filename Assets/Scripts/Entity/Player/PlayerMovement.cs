using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useMovementBounds = true;
    [SerializeField] private Vector2 boundsMin = new Vector2(-20f, -12f);
    [SerializeField] private Vector2 boundsMax = new Vector2(20f, 12f);

    private void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input == Vector2.zero) return;

        Vector3 next = transform.position + (Vector3)(input.normalized * moveSpeed * Time.deltaTime);

        if (useMovementBounds)
        {
            next.x = Mathf.Clamp(next.x, boundsMin.x, boundsMax.x);
            next.y = Mathf.Clamp(next.y, boundsMin.y, boundsMax.y);
        }

        transform.position = next;
    }

    public void ConfigureMovementBounds(bool useBounds, Vector2 min, Vector2 max)
    {
        useMovementBounds = useBounds;

        if (max.x > min.x && max.y > min.y)
        {
            boundsMin = min;
            boundsMax = max;
        }
    }

    public void ConfigureMoveSpeed(float speed)
    {
        if (speed > 0f)
            moveSpeed = speed;
    }

    private void OnDrawGizmos()
    {
        if (!useMovementBounds) return;
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) / 2f, (boundsMin.y + boundsMax.y) / 2f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
