using UnityEngine;
using System.Collections.Generic;

public abstract class OrbitWeaponBase : WeaponBase
{
    [SerializeField] protected float radius = 2.5f;
    [SerializeField] protected float rotateSpeed = 120f;
    [SerializeField] protected int weaponCount = 2;

    protected readonly List<Transform> weapons = new();

    protected virtual void Update()
    {
        transform.position = owner.position;
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    protected abstract Transform CreateWeapon();

    protected void RebuildWeapons()
    {
        foreach (Transform w in weapons)
            Destroy(w.gameObject);

        weapons.Clear();

        float angleStep = 360f / weaponCount;

        for (int i = 0; i < weaponCount; i++)
        {
            float angle = angleStep * i;
            Vector3 pos = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

            Transform weapon = CreateWeapon();
            weapon.SetParent(transform);
            weapon.localPosition = pos;

            weapons.Add(weapon);
        }
    }
}
