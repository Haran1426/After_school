using UnityEngine;

public class BladeOrbitWeapon : OrbitWeaponBase
{
    [SerializeField] private Blade bladePrefab;

    private void Start()
    {
        Init(transform.parent);
        RebuildWeapons();
    }

    protected override Transform CreateWeapon()
    {
        Blade blade = Instantiate(bladePrefab);
        return blade.transform;
    }

    protected override void OnLevelUp()
    {
        weaponCount++;
        RebuildWeapons();
    }
}
