using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool hasJumped = false;
    private BTNode root;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        var chaseNode = new ActionNode(ChasePlayer);
        var attackNode = new ActionNode(AttackPlayer);
        var idleNode = new ActionNode(Idle);

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
            idleNode
        });
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        root.Evaluate();
    }

    // --- Node Functions ---

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
        Vector2 targetPos = new Vector2(player.position.x, transform.position.y); // hanya bergerak horizontal
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        return NodeState.Running;
    }

    NodeState AttackPlayer()
    {
        Debug.Log("Enemy menyerang!");
        return NodeState.Success;
    }

    NodeState Idle()
    {
        return NodeState.Running;
    }

    NodeState CheckObstacle()
    {
    if (!isGrounded)
    {
        return NodeState.Failure;
    }

    Vector2 direction = player.position.x > transform.position.x ? Vector2.right : Vector2.left;
    Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.5f); // tengah musuh
    Vector2 size = new Vector2(0.5f, 1f); // sesuaikan ukuran enemy

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