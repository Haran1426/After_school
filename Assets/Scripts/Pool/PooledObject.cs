using UnityEngine;
using System.Collections.Generic;


public interface IPoolable
{
    void OnSpawned();
    void OnDespawned();
}
public enum PoolType
{
    EnemyMelee,
    EnemyRange,
    Bullet,
    Effect,
    ExpOrb
}

public sealed class ObjectPool
{
    private readonly Stack<PooledObject> stack = new();
    private readonly Transform root;
    private readonly GameObject prefab;

    public ObjectPool(GameObject prefab, Transform parent, int prewarm)
    {
        this.prefab = prefab;

        root = new GameObject($"{prefab.name}_Pool").transform;
        root.SetParent(parent);

        for (int i = 0; i < prewarm; i++)
        {
            var po = Create();
            Release(po);
        }
    }

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        var po = stack.Count > 0 ? stack.Pop() : Create();

        var t = po.transform;
        t.SetPositionAndRotation(pos, rot);

        po.gameObject.SetActive(true);
        po.OnSpawned();

        return po.gameObject;
    }

    public void Release(PooledObject po)
    {
        po.OnDespawned();

        po.gameObject.SetActive(false);
        po.transform.SetParent(root);
        stack.Push(po);
    }

    private PooledObject Create()
    {
        var go = Object.Instantiate(prefab, root);
        var po = go.GetComponent<PooledObject>();
        if (po == null) po = go.AddComponent<PooledObject>();
        po.Bind(this);
        return po;
    }
}

public sealed class PooledObject : MonoBehaviour
{
    private ObjectPool owner;
    private IPoolable[] poolables;

    public void Bind(ObjectPool pool)
    {
        owner = pool;
        poolables = GetComponents<IPoolable>();
    }

    public void ReturnToPool()
    {
        if (owner != null) owner.Release(this);
        else gameObject.SetActive(false);
    }

    internal void OnSpawned()
    {
        for (int i = 0; i < poolables.Length; i++)
            poolables[i].OnSpawned();
    }

    internal void OnDespawned()
    {
        for (int i = 0; i < poolables.Length; i++)
            poolables[i].OnDespawned();
    }
}
