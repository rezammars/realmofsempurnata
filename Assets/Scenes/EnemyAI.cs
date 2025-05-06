using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 5f;
    public int damageToPlayer = 1;
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isGrounded;
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

        var chaseNode = new ActionNode(ChasePlayer);
        var attackNode = new ActionNode(AttackPlayer);
        var patrolNode = new ActionNode(Patrol);

        var chaseAndAttack = new SequenceNode(new System.Collections.Generic.List<BTNode> {
            new ActionNode(CanSeePlayer),
            new SelectorNode(new System.Collections.Generic.List<BTNode> {
                new ActionNode(CheckObstacle),
                chaseNode
            }),
            new ActionNode(IsCloseToPlayer),
            attackNode
        });

        root = new SelectorNode(new System.Collections.Generic.List<BTNode> {
            chaseAndAttack,
            patrolNode
        });
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        root.Evaluate();
    }

    // --- Behavior Tree Node Methods ---

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
        {
            return NodeState.Failure;
        }
        
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Enemy menyerang!");

        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(damageToPlayer);

            playerMovement.ApplySlow(2f, 3f);
            Debug.Log("Player terkena debuff slow dari serangan enemy.");
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
        if (!isGrounded) return NodeState.Failure;

        Vector2 direction = player.position.x > transform.position.x ? Vector2.right : Vector2.left;
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 size = new Vector2(0.5f, 1f);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, 0.6f, groundLayer);
        Debug.DrawRay(origin, direction * 0.6f, Color.red);

        if (hit.collider != null && !hasJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumped = true;
            return NodeState.Success;
        }

        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            hasJumped = false;
        }

        return NodeState.Failure;
    }
}
