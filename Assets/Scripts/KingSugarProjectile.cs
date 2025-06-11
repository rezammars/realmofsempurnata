using UnityEngine;

public class KingSugarProjectile : MonoBehaviour
{
    public int damage = 1;
    void Start() => Destroy(gameObject, 5f);
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player kena peluru!");
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
