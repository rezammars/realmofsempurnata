using System.Collections.Generic;
using UnityEngine;

public class ColaCannonAI : MonoBehaviour
{
    [Header("Player & Deteksi")]
    public Transform player;
    public float detectionRange = 8f;
    public float attackCooldown = 2f;
    public int damageToPlayer = 1;

    [Header("Peluru")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 5f;

    [Header("Health")]
    public int maxHP = 1;

    private int currentHP;
    private bool isDead = false;
    private float lastAttackTime = -Mathf.Infinity;
    private BTNode root;
    private SpriteRenderer sr;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();

        var attackNode = new SequenceNode(new List<BTNode> {
            new ActionNode(CanSeePlayer),
            new ActionNode(ShootAtPlayer)
        });

        root = new SelectorNode(new List<BTNode> {
            attackNode
        });
    }

    void Update()
    {
        if (!isDead)
        {
            root.Evaluate();
        }
    }

    // --- Behavior Tree Methods ---

    NodeState CanSeePlayer()
    {
        if (player == null) return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, player.position);
        return (dist <= detectionRange) ? NodeState.Success : NodeState.Failure;
    }

    NodeState ShootAtPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return NodeState.Failure;

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position - firePoint.position).normalized;

            ColaProjectile cola = bullet.GetComponent<ColaProjectile>();
            if (cola != null)
            {
                cola.SetDirection(direction);
                cola.speed = projectileSpeed;
                cola.lifetime = projectileLifetime;
                cola.damage = damageToPlayer;
            }

            sr.flipX = direction.x < 0; 
        }

        lastAttackTime = Time.time;
        return NodeState.Success;
    }

    // --- Kematian saat diinjak oleh GameObject tertentu ---

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
        Debug.Log("Cola Cannon mati!");

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        Destroy(gameObject, 0.5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // Deteksi berdasarkan nama GameObject atau struktur hirarki
        if (collision.gameObject.name.ToLower().Contains("foot"))
        {
            TakeDamage(1);
        }
    }
}