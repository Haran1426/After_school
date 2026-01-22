using UnityEngine;
using System.Collections.Generic;

public class MagicAttackBase : WeaponBase
{
    [SerializeField] protected int weaponCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnLevelUp()
    {
        weaponCount++;
    }
}
