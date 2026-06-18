using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private float lifeTime = 0.5f;
    private float moveSpeed = 2f;
    private float timer;
    private Color defaultColor = Color.white;

    private void Awake()
    {
        if (text != null)
            defaultColor = text.color;
    }

    public void Init(float damage)
    {
        InitText(damage.ToString(), defaultColor);
    }

    public void InitText(string message, Color color)
    {
        text.text = message;
        text.color = color;
        timer = 0f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            GetComponent<PooledObject>().ReturnToPool();
    }
}
