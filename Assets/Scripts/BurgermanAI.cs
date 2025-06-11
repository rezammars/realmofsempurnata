using System.Collections.Generic;
using UnityEngine;

public class BurgermanAI : MonoBehaviour
{
    [Header("Player & Deteksi")]
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 5f;
    public int damageToPlayer = 1;

    [Header("Patrol & Movement")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public float jumpForce = 5f;

    [Header("Health")]
    public int maxHP = 1;

    private int currentHP;
    private bool isDead = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool hasJumped = false;
    private Vector2 patrolStartPoint;
    private bool movingRight = true;
    private float lastAttackTime = -Mathf.Infinity;
    private BTNode root;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        patrolStartPoint = transform.position;
        currentHP = maxHP;

        var chaseNode = new ActionNode(ChasePlayer);
        var attackNode = new ActionNode(AttackPlayer);
        var patrolNode = new ActionNode(Patrol);

        var chaseAndAttack = new SequenceNode(new List<BTNode> {
            new ActionNode(CanSeePlayer),
            new SelectorNode(new List<BTNode> {
                new ActionNode(CheckObstacle),
                chaseNode
            }),
            new ActionNode(IsCloseToPlayer),
            attackNode
        });

        root = new SelectorNode(new List<BTNode> {
            chaseAndAttack,
            patrolNode
        });
    }

    void Update()
    {
        if (isDead) return;

        root.Evaluate();
    }

    // --- Behavior Tree Methods ---

    NodeState CanSeePlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        return (dist <= detectionRange) ? NodeState.Success : NodeState.Failure;
    }

    NodeState IsCloseToPlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        return (dist <= attackRange) ? NodeState.Success : NodeState.Failure;
    }

    NodeState ChasePlayer()
    {
        Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        sr.flipX = player.position.x < transform.position.x;
        return NodeState.Running;
    }

    NodeState AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return NodeState.Failure;

        rb.linearVelocity = Vector2.zero;
        Debug.Log("Burgerman menyerang!");

        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(damageToPlayer);
            playerMovement.ApplySlow(2f, 3f);
        }

        lastAttackTime = Time.time;
        return NodeState.Success;
    }

    NodeState Patrol()
    {
        float direction = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        sr.flipX = direction < 0;

        if (movingRight && transform.position.x >= patrolStartPoint.x + patrolDistance)
            movingRight = false;
        else if (!movingRight && transform.position.x <= patrolStartPoint.x - patrolDistance)
            movingRight = true;

        return NodeState.Running;
    }

    NodeState CheckObstacle()
    {
        Vector2 direction = player.position.x > transform.position.x ? Vector2.right : Vector2.left;
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 size = new Vector2(0.5f, 1f);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, 0.6f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(origin, direction * 0.6f, Color.red);

        if (hit.collider != null && !hasJumped && hit.collider.CompareTag("Platform"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumped = true;
            return NodeState.Success;
        }

        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            hasJumped = false;
        }

        return NodeState.Failure;
    }

    // --- Kematian saat diinjak ---

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
        Debug.Log("Burgerman mati!");

        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;

        Destroy(gameObject, 0.5f);
    }
}
