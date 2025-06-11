using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public KingSugarAI kingSugarAI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            kingSugarAI.playerInAirZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            kingSugarAI.playerInAirZone = false;
        }
    }
}
