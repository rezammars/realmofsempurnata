using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
     public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private float originalMoveSpeed;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Camera maincamera;

    private Coroutine slowCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maincamera = Camera.main;
        originalMoveSpeed = moveSpeed;
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

        LimitCharacterMovement();
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
}
