using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    [SerializeField] private Knife knifePrefab;
    [SerializeField] private float fireInterval = 0.6f;
    [SerializeField] private float range = 8f;

    private float nextFireTime;


    private void Awake()
    {
        owner = transform;
    }
    private void Update()
    {
        Debug.Log(owner);
        if (owner == null) return;
        if (Time.time < nextFireTime) return;

        EnemyBase target = FindNearestEnemy();
        if (target == null) return;

        Fire(target);
        nextFireTime = Time.time + fireInterval;
    }

    EnemyBase FindNearestEnemy()
    {
        float bestDist = range;
        EnemyBase best = null;

        for (int i = 0; i < EnemyRegistry.All.Count; i++)
        {
            var e = EnemyRegistry.All[i];
            if (e == null || e.IsDead) continue;

            float dist = Vector2.Distance(owner.position, e.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = e;
            }
        }

        return best;
    }

    void Fire(EnemyBase target)
    {
        var go = PoolManager.Instance.Spawn(PoolType.Knife, owner.position, Quaternion.identity);

        if (go == null) return;

        var knife = go.GetComponent<Knife>();
        knife.Init(target.transform.position);
    }

    protected override void OnLevelUp()
    {
        fireInterval *= 0.9f;
    }
}