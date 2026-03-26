using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(float damage, Vector3 position) // 적이 맞았을 때 데미지 텍스트 생성
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
}