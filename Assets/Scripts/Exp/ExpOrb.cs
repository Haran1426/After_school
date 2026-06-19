using UnityEngine;

public class ExpOrb : MonoBehaviour, IPoolable
{
    public int expValue = 1;
    [SerializeField, Min(0.1f)] private float magnetDistance = 3.2f;
    [SerializeField, Min(0.05f)] private float absorbDistance = 0.2f;
    [SerializeField, Min(0.1f)] private float moveSpeed = 8f;
    [SerializeField, Min(0f)] private float acceleration = 18f;
    [SerializeField, Min(0.02f)] private float farSmoothTime = 0.24f;
    [SerializeField, Min(0.02f)] private float nearSmoothTime = 0.055f;
    [SerializeField, Min(0f)] private float orbitStrength = 0.65f;
    [SerializeField, Min(1f)] private float nearPopScale = 1.35f;

    Transform player;
    PlayerExp playerExp;
    Player playerEntity;
    PooledObject pooled;
    Vector3 pullVelocity;
    float magnetAge;
    float nextTrailTime;
    float wobbleSeed;
    Vector3 baseScale;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();
        baseScale = transform.localScale;
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
        {
            magnetAge = 0f;
            pullVelocity = Vector3.Lerp(pullVelocity, Vector3.zero, Time.deltaTime * 8f);
            UpdateIdleVisual();
            return;
        }

        magnetAge += Time.deltaTime;
        PullTowardPlayer(dist, activeMagnetDistance);
        UpdateMagnetVisual(dist, activeMagnetDistance);

        if (Vector3.Distance(transform.position, player.position) <= absorbDistance)
            Absorb();
    }

    public void OnSpawned()
    {
        pullVelocity = Vector3.zero;
        magnetAge = 0f;
        nextTrailTime = 0f;
        wobbleSeed = Random.Range(0f, 100f);
        transform.localScale = baseScale;
        CachePlayer();
    }

    public void OnDespawned()
    {
        pullVelocity = Vector3.zero;
        magnetAge = 0f;
        transform.localScale = baseScale;
    }

    private void Absorb()
    {
        GameJuiceFX.ExpBurst(transform.position);

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

    private void UpdateIdleVisual()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 5.5f + transform.position.x) * 0.08f;
        transform.localScale = baseScale * pulse;
    }

    private void PullTowardPlayer(float distance, float activeMagnetDistance)
    {
        Vector3 toPlayer = player.position - transform.position;
        float magnetT = 1f - Mathf.Clamp01(distance / activeMagnetDistance);
        float ease = Mathf.SmoothStep(0f, 1f, magnetT);
        Vector3 target = player.position;

        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Vector3 tangent = new Vector3(-toPlayer.y, toPlayer.x, 0f).normalized;
            float orbit = Mathf.Sin((Time.time + wobbleSeed) * Mathf.Lerp(5.5f, 11f, ease));
            target += tangent * orbit * orbitStrength * (1f - ease) * Mathf.Clamp01(magnetAge * 5f);
        }

        float smoothTime = Mathf.Lerp(farSmoothTime, nearSmoothTime, ease);
        float maxSpeed = moveSpeed + acceleration * ease;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref pullVelocity, smoothTime, maxSpeed, Time.deltaTime);

        if (ease > 0.78f)
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * 0.75f * ease * Time.deltaTime);
    }

    private void UpdateMagnetVisual(float distance, float activeMagnetDistance)
    {
        float t = 1f - Mathf.Clamp01(distance / activeMagnetDistance);
        float pulse = 1f + Mathf.Sin((Time.time + wobbleSeed) * Mathf.Lerp(8f, 16f, t)) * Mathf.Lerp(0.1f, 0.22f, t);
        pulse *= Mathf.Lerp(1f, nearPopScale, Mathf.SmoothStep(0.65f, 1f, t));
        transform.localScale = baseScale * pulse;

        if (Time.time >= nextTrailTime)
        {
            nextTrailTime = Time.time + Mathf.Lerp(0.1f, 0.035f, t);
            GameJuiceFX.ExpTrail(transform.position);
        }
    }
}
