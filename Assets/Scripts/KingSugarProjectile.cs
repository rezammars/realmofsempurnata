using UnityEngine;

public class KingSugarProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;
    public float slowDuration = 3f;
    public float slowAmount = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                player.ApplySlow(slowAmount, slowDuration);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
