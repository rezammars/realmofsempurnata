using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    public int maxHP = 100;
    public PlayerUI playerUI;


    private float originalMoveSpeed;
    private float originalJumpForce;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Camera maincamera;
    private float originalLightRadius;
    private Coroutine lightBoostCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine powerJumpCoroutine;
    private Coroutine invincibilityCoroutine;
    private int currentHP;
    private bool isInvincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maincamera = Camera.main;
        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce;
        currentHP = maxHP;

        if (playerLight != null)
            originalLightRadius = playerLight.pointLightOuterRadius;

        if (playerUI != null)
        playerUI.UpdateHP(currentHP, maxHP);

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

    public void TakeDamage(int amount)
    {
        if (isInvincible)
        {
            Debug.Log("Player kebal! Tidak menerima damage.");
            return;
        }

        currentHP -= amount;
        Debug.Log("HP Player: " + currentHP);

        if (playerUI != null)
        playerUI.UpdateHP(currentHP, maxHP);
        
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player mati!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void ActivatePowerJump()
    {
        if (powerJumpCoroutine != null)
            StopCoroutine(powerJumpCoroutine);

        powerJumpCoroutine = StartCoroutine(PowerJumpRoutine());
    }

    IEnumerator PowerJumpRoutine()
    {
        jumpForce = originalJumpForce + 4f;
        Debug.Log("Power Jump aktif!");
        yield return new WaitForSeconds(5f);
        jumpForce = originalJumpForce;
        Debug.Log("Power Jump berakhir.");
    }

    public void ActivateInvincibility(float duration)
    {
        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);

        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        Debug.Log("Player sekarang kebal!");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Player tidak kebal lagi.");
    }
}
