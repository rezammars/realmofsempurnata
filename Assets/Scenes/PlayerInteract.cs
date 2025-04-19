using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 1.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Interactable"))
                {
                    var obj = hit.GetComponent<ObjectInteract>();
                    if (obj != null)
                    {
                        obj.DoInteraction();
                        break;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}