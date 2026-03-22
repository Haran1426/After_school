using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private float lifeTime = 0.5f;
    private float moveSpeed = 2f;
    private float timer;

    public void Init(float damage)
    {
        text.text = damage.ToString();
        timer = 0f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            gameObject.SetActive(false);
    }
}