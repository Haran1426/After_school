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
        if (owner == null)
            return;

        transform.position = owner.position;
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
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
            Vector3 dir = Quaternion.Euler(0f, 0f, angle) * Vector3.right;
            Vector3 pos = dir * radius;

            Transform weapon = CreateWeapon();
            weapon.SetParent(transform);
            weapon.localPosition = pos;
            weapon.localRotation = Quaternion.identity;

            weapons.Add(weapon);
        }
    }
}
