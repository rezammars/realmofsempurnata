using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 4f;
    private float originalMoveSpeed;
    private float originalJumpForce;

    [Header("Jump Control")]
    public float jumpCooldown = 0.2f;
    private float lastJumpTime = -10f;

    [Header("Attack")]
    public float attackRange = 1f;
    public int attackDamage = 1;
    public Transform attackPoint;

    [Header("Player Stats")]
    public int maxHP = 100;
    public PlayerUI playerUI;
    public float fallThresholdY = -7f;

    [Header("Light Effect")]
    public Light2D playerLight;
    private float originalLightRadius;

    private Rigidbody2D rb;
    private Camera maincamera;

    private int currentHP;
    private bool isInvincible = false;

    private Coroutine lightBoostCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine jumpDebuffCoroutine;
    private Coroutine powerJumpCoroutine;
    private Coroutine invincibilityCoroutine;

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
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && Time.time >= lastJumpTime + jumpCooldown)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpTime = Time.time;
        }

        if (transform.position.y < fallThresholdY)
        {
            Debug.Log("Player jatuh ke jurang!");
            Die();
        }

        LimitCharacterMovement();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (transform.position.y > other.transform.position.y + 0.3f)
            {
                bool damaged = false;

                if (other.TryGetComponent<BurgermanAI>(out var burger))
                {
                    burger.TakeDamage(1);
                    damaged = true;
                }
                else if (other.TryGetComponent<ColaCannonAI>(out var cola))
                {
                    cola.TakeDamage(1);
                    damaged = true;
                }
                else if (other.TryGetComponent<KingSugarAI>(out var kingSugar))
                {
                    kingSugar.TakeDamage(1);
                    damaged = true;
                }
                if (damaged)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.5f);
                }
            }
        }
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

    // === Cola Cannon Effect ===
    public void ApplySlow(float slowSpeed, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        Debug.Log($"Player terkena slow: kecepatan jadi {slowSpeed} selama {duration} detik");
        slowCoroutine = StartCoroutine(SlowCoroutine(slowSpeed, duration));
    }

    IEnumerator SlowCoroutine(float slowSpeed, float duration)
    {
        moveSpeed = slowSpeed;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    // === Burgerman Effect ===
    public void ApplyJumpDebuff(float multiplier, float duration)
    {
        if (jumpDebuffCoroutine != null)
            StopCoroutine(jumpDebuffCoroutine);

        Debug.Log($"Player terkena jump debuff selama {duration} detik");
        jumpDebuffCoroutine = StartCoroutine(JumpDebuffRoutine(multiplier, duration));
    }

    IEnumerator JumpDebuffRoutine(float multiplier, float duration)
    {
        jumpForce = originalJumpForce * multiplier;
        yield return new WaitForSeconds(duration);
        jumpForce = originalJumpForce;
    }

    // === Light Boost Power Up ===
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

    // === Power Jump Power Up ===
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

    // === Invincibility Power Up ===
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