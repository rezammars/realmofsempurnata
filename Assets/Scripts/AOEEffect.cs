using System.Collections;
using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    public float duration = 1f;
    public float damageDelay = 0.1f;
    public int damage = 2;
    public float slowDuration = 2f;
    public float slowAmount = 2f;
    public LayerMask playerLayer;

    private void Start()
    {
        StartCoroutine(DoAOE());
    }

    IEnumerator DoAOE()
    {
        yield return new WaitForSeconds(damageDelay);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            Movement player = hit.GetComponent<Movement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                player.ApplySlow(slowDuration, slowAmount);
            }
        }

        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
    }
}