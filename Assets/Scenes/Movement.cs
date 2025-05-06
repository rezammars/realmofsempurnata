using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Movement : MonoBehaviour
{
     public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    public Light2D playerLight;
    public float attackRange = 1f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;
    public Transform attackPoint;


    private float originalMoveSpeed;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Camera maincamera;
    private float originalLightRadius;
    private Coroutine lightBoostCoroutine;
    private Coroutine slowCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maincamera = Camera.main;
        originalMoveSpeed = moveSpeed;

        if (playerLight != null)
            originalLightRadius = playerLight.pointLightOuterRadius;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }

        LimitCharacterMovement();
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void LimitCharacterMovement()
    {
        float cameraLeftBound = maincamera.transform.position.x - maincamera.orthographicSize * maincamera.aspect;
        float cameraRightBound = maincamera.transform.position.x + maincamera.orthographicSize * maincamera.aspect;
        float clampedX = Mathf.Clamp(transform.position.x, cameraLeftBound, cameraRightBound);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    public void ApplySlow(float slowSpeed, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        
        Debug.Log($"Player terkena efek slow: kecepatan {slowSpeed} selama {duration} detik");
        slowCoroutine = StartCoroutine(SlowCoroutine(slowSpeed, duration));
    }

    IEnumerator SlowCoroutine(float slowSpeed, float duration)
    {
        moveSpeed = slowSpeed;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    public void BoostLight()
    {
        if (lightBoostCoroutine != null)
            StopCoroutine(lightBoostCoroutine);

        lightBoostCoroutine = StartCoroutine(BoostLightRoutine());
    }

    IEnumerator BoostLightRoutine()
    {
       playerLight.pointLightOuterRadius = originalLightRadius + 4f;
        yield return new WaitForSeconds(5f);
        playerLight.pointLightOuterRadius = originalLightRadius;
    }
}
