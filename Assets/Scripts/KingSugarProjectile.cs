using UnityEngine;

public class KingSugarProjectile : MonoBehaviour
{
     public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement movement = other.GetComponent<Movement>();
            if (movement != null)
            {
                movement.TakeDamage(damage);
                movement.ApplySlow(2f, 3f);
            }
            Destroy(gameObject);
        }
    }
}
