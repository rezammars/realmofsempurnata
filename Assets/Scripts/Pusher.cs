using UnityEngine;

public class Pusher : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool canBePushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DisablePush(); // default: tidak bisa didorong
    }

    public void EnablePush()
    {
        canBePushed = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DisablePush()
    {
        canBePushed = false;
        rb.bodyType = RigidbodyType2D.Static; // tidak bisa bergerak
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canBePushed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Bisa tambahkan efek atau dorongan manual jika dibutuhkan
        }
    }
}
