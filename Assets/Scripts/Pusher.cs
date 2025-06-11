using UnityEngine;

public class Pusher : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool canBePushed = false;
    private int currentHP;
    public int damageToEnemy = 1;
    public float damageSpeedThreshold = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DisablePush();
    }

    public void EnablePush()
    {
        canBePushed = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DisablePush()
    {
        canBePushed = false;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log("Pushable menerima damage, sisa HP: " + currentHP);

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
         if (collision.gameObject.CompareTag("Player"))
    {
        if (collision.gameObject.TryGetComponent(out Movement movement))
        {
            if (movement.canPush)
            {
                EnablePush();
            }
        }
    }
    }
}
