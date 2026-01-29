using UnityEngine;

public class EnemyLifeHook : MonoBehaviour, IPoolable
{
    private EnemySpawner spawner;

    public void Bind(EnemySpawner owner)
    {
        spawner = owner;
    }

    public void OnSpawned()
    {

    }

    public void OnDespawned()
    {
        if (spawner != null)
            spawner.NotifyEnemyDead();
    }
}
