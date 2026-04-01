using UnityEngine;

public class EnemyLifeHook : MonoBehaviour, IPoolable
{
    private IAliveCounter counter;

    public void Bind(IAliveCounter owner)
    {
        counter = owner;
    }

    public void OnSpawned() { }

    public void OnDespawned()
    {
        counter?.NotifyEnemyDead();
    }
}
