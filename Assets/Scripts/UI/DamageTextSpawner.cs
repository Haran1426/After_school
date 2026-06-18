using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(float damage, Vector3 position)
    {
        var obj = GameRoot.Instance.Pool.Spawn(
            PoolType.DamageText,
            position,
            Quaternion.identity
        );

        if (obj == null) return;

        var damageText = obj.GetComponent<DamageText>();
        damageText.Init(damage);
    }

    public void SpawnText(string message, Vector3 position, Color color)
    {
        var obj = GameRoot.Instance.Pool.Spawn(
            PoolType.DamageText,
            position,
            Quaternion.identity
        );

        if (obj == null) return;

        var damageText = obj.GetComponent<DamageText>();
        damageText.InitText(message, color);
    }
}
