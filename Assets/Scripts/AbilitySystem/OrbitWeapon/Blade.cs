using UnityEngine;
using System.Collections.Generic;

public class Blade : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    public LayerMask EnemyLayer;
    float FindRage = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        //EnemyHealth enemy = other.GetComponent<EnemyHealth>(); 데미지 추가 예정
        //if (enemy != null)
        //    enemy.TakeDamage(damage);
    }
    private void FixedUpdate()
    {
        var EnemyObj = Physics.OverlapSphere(transform.position, FindRage, EnemyLayer);
        print(EnemyObj.Length);
    }

}
