using System.Collections.Generic;
using UnityEngine;

public class KingSugarAI : MonoBehaviour
{
    [Header("Player & Detection")]
    public Transform player;
    public float detectionRange = 10f;
    public float aoeRange = 2f;
    public float projectileCooldown = 5f;
    public float aoeCooldown = 5f;
    public int damageAOE = 2;
    public int damageProjectile = 1;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Movement")]
    public float moveSpeed = 1.5f;

    [Header("Health")]
    public int maxHP = 20;

    private int currentHP;
    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float lastAOETime = -Mathf.Infinity;
    private float lastShootTime = -Mathf.Infinity;

    private BTNode root;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHP = maxHP;

        var attackSelector = new SelectorNode(new List<BTNode>
        {
            new SequenceNode(new List<BTNode> {
                new ActionNode(PlayerIsNearOnGround),
                new ActionNode(AOEStrike)
            }),
            new SequenceNode(new List<BTNode> {
                new ActionNode(PlayerIsFarOnPlatform),
                new ActionNode(ShootProjectile)
            })
        });

        var moveToPlayer = new ActionNode(MoveTowardPlayer);
        var mainBehavior = new SelectorNode(new List<BTNode>
        {
            new ActionNode(CanSeePlayer),
            attackSelector,
            moveToPlayer
        });

        root = mainBehavior;
    }

    void Update()
    {
        if (isDead) return;
        root.Evaluate();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
        
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 playerBottom = new Vector2(transform.position.x, transform.position.y - 0.5f);

            if (playerBottom.y > contactPoint.y)
            {
            
                KingSugarAI enemy = collision.gameObject.GetComponent<KingSugarAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                }
            }
        }
    }


    // --- Behavior Tree Methods ---

    NodeState CanSeePlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        return (dist <= detectionRange) ? NodeState.Success : NodeState.Failure;
    }

    NodeState PlayerIsNearOnGround()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        bool isGround = IsPlayerOnGround();
        return (dist <= aoeRange && isGround) ? NodeState.Success : NodeState.Failure;
    }

    NodeState PlayerIsFarOnPlatform()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        bool isPlatform = IsPlayerOnPlatform();
        return (dist > aoeRange && isPlatform) ? NodeState.Success : NodeState.Failure;
    }

    NodeState AOEStrike()
    {
        if (Time.time - lastAOETime < aoeCooldown)
            return NodeState.Failure;

        Debug.Log("King Sugar AOE Strike!");
        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(damageAOE);
            playerMovement.ApplySlow(2f, 2f);
        }

        lastAOETime = Time.time;
        return NodeState.Success;
    }

    NodeState ShootProjectile()
    {
        if (Time.time - lastShootTime < projectileCooldown)
            return NodeState.Failure;

        Debug.Log("King Sugar shoots!");
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        lastShootTime = Time.time;
        return NodeState.Success;
    }

    NodeState MoveTowardPlayer()
    {
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        sr.flipX = player.position.x < transform.position.x;
        return NodeState.Running;
    }

    // --- Utility Checks ---

    bool IsPlayerOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

    bool IsPlayerOnPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, 0.2f, LayerMask.GetMask("Platform"));
        return hit.collider != null;
    }

    // --- Damage & Death ---

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
        Debug.Log("King Sugar defeated!");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;
        Destroy(gameObject, 1f);
    }
}
