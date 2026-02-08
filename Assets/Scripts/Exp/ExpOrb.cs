using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int expValue = 1;
    public float absorbDistance = 1.2f;
    public float moveSpeed = 8f;

    Transform player;
    PlayerExp playerExp;
    PooledObject pooled;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) return;

        player = p.transform;
        playerExp = p.GetComponent<PlayerExp>();
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > absorbDistance)
            return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed 
            * Time.deltaTime);

        if (dist < 0.1f)
            Absorb();
    }

    private void Absorb()
    {
        if (playerExp != null)
            playerExp.AddExp(expValue);

        var po = GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else gameObject.SetActive(false);
    }
}