using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class KingSugarAI : MonoBehaviour
{
    [Header("Target & Range")]
    public Transform player;
    public float aoeRange = 2f;
    public float chaseRange = 5f;

    [Header("AOE Settings")]
    public Transform aoeCenter;
    public float aoeRadius = 1.8f;
    public float aoeCooldown = 4f;
    public int aoeDamage = 2;
    public LayerMask playerLayer;
    public GameObject aoePrefab;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileCooldown = 3f;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Health Settings")]
    public int maxHP = 10;
    private int currentHP;

    private float lastAOETime = -999f;
    private float lastProjectileTime = -999f;

    public bool playerInAirZone = false;
    private Rigidbody2D rb;
    private BTNode treeRoot;

    void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        BuildTree();
    }

    void Update()
    {
        if (currentHP > 0)
            treeRoot?.Evaluate();
    }

    void BuildTree()
    {
        var playerInAOERange = new ActionNode(() =>
        {
            float distance = Vector2.Distance(transform.position, player.position);
            return (!playerInAirZone && distance <= aoeRange) ? NodeState.Success : NodeState.Failure;
        });

        var doAOE = new ActionNode(() =>
        {
            if (!IsAOEOnCooldown())
            {
                ExecuteAOE();
                lastAOETime = Time.time;
                return NodeState.Success;
            }
            return NodeState.Failure;
        });

        var aoeSequence = new SequenceNode(new List<BTNode> {
            playerInAOERange,
            doAOE
        });

        var playerInChaseRange = new ActionNode(() =>
        {
            float distance = Vector2.Distance(transform.position, player.position);
            return (!playerInAirZone && distance <= chaseRange && distance > aoeRange)
                ? NodeState.Success
                : NodeState.Failure;
        });

        var chase = new ActionNode(() =>
        {
            ChasePlayer();
            return NodeState.Running;
        });

        var chaseSequence = new SequenceNode(new List<BTNode> {
            playerInChaseRange,
            chase
        });

        var projectileAttack = new ActionNode(() =>
        {
            if (playerInAirZone)
            {
                rb.linearVelocity = Vector2.zero;
                if (Time.time - lastProjectileTime >= projectileCooldown)
                {
                    ShootProjectile();
                    lastProjectileTime = Time.time;
                }
                return NodeState.Running;
            }
            return NodeState.Failure;
        });

        var idle = new ActionNode(() =>
        {
            rb.linearVelocity = Vector2.zero;
            return NodeState.Running;
        });

        treeRoot = new SelectorNode(new List<BTNode> {
            aoeSequence,
            chaseSequence,
            projectileAttack,
            idle
        });
    }

    public void ExecuteAOE()
    {
        Debug.Log("AOE Attack!");

        Instantiate(aoePrefab, aoeCenter.position, Quaternion.identity);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(aoeCenter.position, aoeRadius, playerLayer);
        foreach (var hit in hits)
        {
            Debug.Log("Player terkena AOE");
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<Movement>()?.TakeDamage(aoeDamage);
            }
        }
    }

    public void ShootProjectile()
    {
        Debug.Log("Shoot Projectile!");

        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj != null)
            rbProj.linearVelocity = direction * 5f;
    }

    public void ChasePlayer()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        FlipToPlayer(dir);
    }

    void FlipToPlayer(float dir)
    {
        Vector3 scale = transform.localScale;
        float originalScaleX = Mathf.Abs(scale.x);

        scale.x = dir < 0 ? -originalScaleX : originalScaleX;
        transform.localScale = scale;
    }

    public bool IsAOEOnCooldown()
    {
        return Time.time - lastAOETime < aoeCooldown;
    }

    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return;

        currentHP -= amount;
        Debug.Log("King Sugar HP: " + currentHP);

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("King Sugar defeated!");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (aoeCenter != null)
            Gizmos.DrawWireSphere(aoeCenter.position, aoeRadius);
    }
}