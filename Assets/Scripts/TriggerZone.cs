using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public KingSugarAI kingSugarAI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            kingSugarAI.playerInAirZone = true;
            Debug.Log("Player MASUK zona udara");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            kingSugarAI.playerInAirZone = false;
            Debug.Log("Player KELUAR dari zona udara");
        }
    }
}
