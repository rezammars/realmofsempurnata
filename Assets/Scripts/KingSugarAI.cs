using UnityEngine;

public class KingSugarAI : MonoBehaviour
{
    [Header("Referensi")]
    public Transform player;
    public GameObject aoeEffect;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Statistik")]
    public int maxHP = 10;
    public float moveSpeed = 2f;
    public float attackCooldown = 5f;
    public float aoeRadius = 3f;
    public int aoeDamage = 2;
    public float projectileSpeed = 5f;

    [Header("Deteksi")]
    public float chaseTriggerRadius = 6f;
    public LayerMask groundLayer;

    private int currentHP;
    private float lastAttackTime;
    private bool isDead = false;
    private bool playerInRange = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (isDead || player == null || !playerInRange) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Gerak mendekati player
        MoveTowardsPlayer();

        // Cek attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (IsPlayerOnPlatform())
                ShootProjectile();
            else if (IsPlayerOnGround() && distance <= aoeRadius)
                DoAOEAttack();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    bool IsPlayerOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 2f);
        return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    bool IsPlayerOnPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 2f);
        return hit.collider != null && hit.collider.CompareTag("Platform");
    }

    void DoAOEAttack()
    {
        lastAttackTime = Time.time;
        Instantiate(aoeEffect, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Movement movement = hit.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.TakeDamage(aoeDamage);
                    movement.ApplySlow(2f, 3f);
                    movement.ApplyJumpDebuff(0.5f, 3f);
                }
            }
        }

        Debug.Log("King Sugar melakukan AOE!");
    }

    void ShootProjectile()
    {
        lastAttackTime = Time.time;
        Vector2 direction = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProj = bullet.GetComponent<Rigidbody2D>();
        if (rbProj != null)
        {
            rbProj.linearVelocity = direction * projectileSpeed;
        }

        Debug.Log("King Sugar menembak proyektil!");
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("King Sugar kalah!");
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
