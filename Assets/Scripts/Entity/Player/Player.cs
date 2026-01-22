using UnityEngine;

public class Player : Entity
{
    public float moveSpeed = 5f;

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        transform.Translate(input.normalized * moveSpeed * Time.deltaTime);
    }

    protected override void Die()
    {
        //GameManager.Instance.GameOver();
    }
}
