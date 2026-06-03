using UnityEngine;

public class ExpOrb : MonoBehaviour, IPoolable
{
    public int expValue = 1;
    [SerializeField, Min(0.1f)] private float magnetDistance = 3.2f;
    [SerializeField, Min(0.05f)] private float absorbDistance = 0.2f;
    [SerializeField, Min(0.1f)] private float moveSpeed = 8f;
    [SerializeField, Min(0f)] private float acceleration = 18f;

    Transform player;
    PlayerExp playerExp;
    Player playerEntity;
    PooledObject pooled;
    float currentSpeed;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();
        CachePlayer();
    }

    private void Update()
    {
        if (player == null)
            CachePlayer();

        if (player == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        float activeMagnetDistance = magnetDistance + (playerEntity != null ? playerEntity.ExpMagnetBonus : 0f);
        if (dist > activeMagnetDistance)
            return;

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

        if (dist <= absorbDistance)
            Absorb();
    }

    public void OnSpawned()
    {
        currentSpeed = moveSpeed * 0.35f;
        CachePlayer();
    }

    public void OnDespawned()
    {
        currentSpeed = 0f;
    }

    private void Absorb()
    {
        if (playerExp != null)
            playerExp.AddExp(expValue);

        if (pooled != null) pooled.ReturnToPool();
        else gameObject.SetActive(false);
    }

    private void CachePlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            player = null;
            playerExp = null;
            playerEntity = null;
            return;
        }

        player = p.transform;
        playerExp = p.GetComponent<PlayerExp>();
        playerEntity = p.GetComponent<Player>();
    }
}
