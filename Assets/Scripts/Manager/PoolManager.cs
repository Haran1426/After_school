using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolSetup
{
    public PoolType type;
    public GameObject prefab;
    public int prewarmCount = 10;
}

public sealed class PoolManager : ManagerBase<PoolManager>
{
    [SerializeField] private List<PoolSetup> setups = new();

    private readonly Dictionary<PoolType, ObjectPool> pools = new();
    private Transform root;

    protected override void Awake()
    {
        base.Awake();

        root = new GameObject("Pools").transform;
        root.SetParent(transform);

        BuildPools();
    }

    private void BuildPools()
    {
        pools.Clear();

        for (int i = 0; i < setups.Count; i++)
        {
            var s = setups[i];
            if (s.prefab == null) continue;

            if (pools.ContainsKey(s.type))
            {
                Debug.LogError($"Duplicate PoolType: {s.type}");
                continue;
            }

            pools.Add(
                s.type,
                new ObjectPool(s.prefab, root, Mathf.Max(0, s.prewarmCount))
            );
        }
    }

    public GameObject Spawn(PoolType type, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(type, out var pool))
            return null;

        return pool.Get(pos, rot);
    }

    public void Despawn(GameObject go)
    {
        if (go == null) return;

        var po = go.GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else go.SetActive(false);
    }
}
