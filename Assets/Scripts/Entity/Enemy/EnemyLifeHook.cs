using UnityEngine;

public class EnemyLifeHook : MonoBehaviour, IPoolable
{
    private IAliveCounter counter;
    private bool isCounted;

    public void Bind(IAliveCounter owner)
    {
        counter = owner;
        isCounted = owner != null;
    }

    public void OnSpawned() { }

    public void OnDespawned()
    {
        if (isCounted)
            counter?.NotifyEnemyDead();

        isCounted = false;
        counter = null;
    }
}
