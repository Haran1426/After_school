using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int level = 1;
    [SerializeField] protected int maxLevel = 8;

    protected Transform owner;

    public virtual void Init(Transform ownerTransform)
    {
        owner = ownerTransform;
    
    
    }
    public void LevelUp()
    {
        if (level >= maxLevel)
            return;

        level++;
        OnLevelUp();
    }
    protected abstract void OnLevelUp();
}
