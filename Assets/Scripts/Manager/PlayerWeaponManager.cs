using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponManager : MonoBehaviour
{
    private Dictionary<System.Type, WeaponBase> ownedWeapons = new();

    public void AddOrUpgradeWeapon(WeaponBase weaponPrefab)
    {
        var type = weaponPrefab.GetType();

        if (!ownedWeapons.TryGetValue(type, out var weapon))
        {
            WeaponBase newWeapon = Instantiate(weaponPrefab, transform);
            newWeapon.Init(transform);
            ownedWeapons.Add(type, newWeapon);
        }
        else
        {
            weapon.LevelUp();
        }
    }
}