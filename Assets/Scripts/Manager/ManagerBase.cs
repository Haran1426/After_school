using UnityEngine;


public interface IManagerInitialize
{
    void Initialize();
}
public abstract class ManagerBase : MonoBehaviour, IManagerInitialize
{
    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        OnInitialize();
        IsInitialized = true;
    }

    protected abstract void OnInitialize();
}
